using NDesk.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using XDMessaging;

namespace Appacker
{
    static class Program
    {
        [DllImport("USER32.DLL")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("kernel32.dll")]
        static extern bool AttachConsole(int dwProcessId);
        private const int ATTACH_PARENT_PROCESS = -1;

        private const string TRY_HELP = "Try `{0} --help' for more information.";
        private const string USAGE = @"Usage: {0} [-r] [-q] <-s ""source_folder""> <-e ""main_exe""> [-d ""save_location""] [-i ""icon_path""] [-a ""launch_arguments""] [-fd ""file_description""]";

        // Flag to keep application running until background packing is finished
        // It's done to print packing progress messages to the console
        private static bool packingFinished = false;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Redirect console output to parent process (cmd.exe)
            // Must be before any calls to Console.WriteLine()
            AttachConsole(ATTACH_PARENT_PROCESS);
            
            string[] args = Environment.GetCommandLineArgs();
            // 1 argument = application is called without arguments
            if (args.Length > 1)
            {
                NoGuiMode(args);
            }
            // Regular application launch
            else
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MainForm());
            }
        }

        static void NoGuiMode(string[] args)
        {
            string sourceAppFolder = null, mainExePath = null, destinationPath = null, customIconPath = null, launchArguments = "", fileDescription = null, password = "";
            bool selfRepackable = false, openUnpackedDir = false, quietPacking = false, isWindowlessUnpacker = false;
            bool showHelp = false;
            MainForm.UnpackDirectory unpackDir = MainForm.UnpackDirectory.Temp;

            // Do all the important things in the try block to be able to execute SendKeys in the finally block regardless of the return point
            try
            {
                // Initialize possible arguments
                var argsParser = new OptionSet() {
                    { "s|src|source-folder|input=",
                        "Directory containing all the files of the target application.",
                        s => sourceAppFolder = s.TrimEnd('\\', '/') },
                    { "e|exe|main-exe=",
                        "Local path to the main executable inside the source app folder. Can be .exe, .bat, .cmd, .lnk or .sh file. This is the application that will launch whenever the packed app extracts itself.",
                        e => mainExePath = e.TrimStart('\\', '/') },
                    { "d|dst|destination|output=",
                        "Location where packed app will be created.",
                        d => destinationPath = d },
                    { "i|ico|icon=",
                        "Custom icon for the packed application.",
                        c => customIconPath = c },
                    { "r|repack|self-repack",
                        "Sets the packed application to refresh itself after the main executable closes. Refreshing adds and replaces files in the packed executable with those created and modified during runtime.",
                        r => selfRepackable = r != null },
                    { "a|args|arguments=",
                        "Arguments to pass to the main executable at every launch.\nYou should add escaped quotes for paths:\n-a \"--some-flag \\\"c:\\file with spaces.txt\\\"\"",
                        a => launchArguments = a ?? "" },
                    { "fd|description|file-description=",
                        "FileDescription string for main executable (used in Windows 'Open with...' menu).",
                        fd => fileDescription = fd },
                    { "o|open-dir",
                        "Open folder with unpacked application in Explorer when you launch packed app.",
                        o => openUnpackedDir = o != null },
                    { "q|quiet|silent",
                        "No progress messages will be shown during the packing process.",
                        q => quietPacking = q != null },
                    { "h|help|?",
                        "Show this message and exit.",
                       v => showHelp = v != null },
                    { "u|udir|unpack-dir=",
                        "Directory where to unpack app files. Can be [temp|desktop|same|ask]. Default is 'temp'.\nIf set 'same', temp directory will be created in the same directory where packed exe is.\nIf set 'ask', it will prompt user before each unpacking.",
                        u => {
                            if (u == "temp")
                                unpackDir = MainForm.UnpackDirectory.Temp;
                            else if (u == "desktop")
                                unpackDir = MainForm.UnpackDirectory.Desktop;
                            else if (u == "same")
                                unpackDir = MainForm.UnpackDirectory.NextToPackedExe;
                            else if (u == "ask")
                                unpackDir = MainForm.UnpackDirectory.AskAtLaunch; } },
                    { "p|pass|password=",
                        "Password for packed application",
                        p => password = p },
                    { "w|windowless",
                        "Makes unpacker daemon completely hidden (console window doesn't appear for a split-second), BUT it can not redirect stdout of a packed application",
                        w => isWindowlessUnpacker = w != null }
                };

                try
                {
                    // Parse arguments
                    argsParser.Parse(args);
                }
                catch (OptionException e)
                {
                    Console.WriteLine("Appacker: " + e.Message);
                    Console.WriteLine(TRY_HELP, args[0]);
                    return;
                }

                // If --help flag is present
                if (showHelp)
                {
                    ShowHelp(argsParser, args[0]);
                    return;
                }
                else
                {
                    // Validate arguments and show error messages if required
                    if (!ValidateArguments(sourceAppFolder, mainExePath, destinationPath, customIconPath, isQuiet: quietPacking))
                    {
                        Console.WriteLine(USAGE, args[0]);
                        Console.WriteLine(TRY_HELP, args[0]);
                        return;
                    }

                    // If destination file was not specified => save package next to the source directory
                    if (destinationPath == null)
                    {
                        destinationPath = Path.Combine(Path.GetDirectoryName(sourceAppFolder), Path.GetFileName(mainExePath));
                    }

                    // == PACKING ==
                    // Subscribe to progress update events
                    if (!quietPacking)
                    {
                        // Position of the concole cursor
                        (int left, int top) firstMessageCursorPos = default;
                        bool firstMessageReceived = false;
                        // Value from ProgressUpdate message
                        int maxValue = 0;
                        object syncObj = new object();

                        MainForm.PackingProgressUpdate += (o, progress) =>
                        {
                            // Multiple messages can try to write to the same spon in the Console, hence lock
                            lock (syncObj)
                            {
                                // This check is kinda crutchy. Using Compatibility mode in XDMessaging may lead to non-sequential progress updates, 
                                // i.e. `5% progress` message can arrive after `8% progress` one. It's not nice, but there's no easy fixes, so i'm gonna leave it as it is,
                                // but having `95%` message arrive after `100%` is ugly and this check for packingFinished fixes at least this situation
                                if (!packingFinished)
                                {
                                    // Remember the position of the first message in Console, so we can write to the same spot 
                                    // and it looks neat and not like a stream of text
                                    if (!firstMessageReceived)
                                    {
                                        firstMessageReceived = true;
                                        firstMessageCursorPos = (Console.CursorLeft, Console.CursorTop);
                                        // Also remember maxValue for the PackingFinished event, because that event doesn't have `100/100` like text. Yeah, another crutch, i know .__.
                                        maxValue = progress.maxValue;
                                    }

                                    // Move cursor and write new progress message on top of the old one
                                    Console.SetCursorPosition(firstMessageCursorPos.left, firstMessageCursorPos.top);
                                    Console.WriteLine($"Packing... {progress.currentValue} / {progress.maxValue} done [{100f * progress.currentValue / progress.maxValue:F2}%]");
                                }
                            }
                        };
                        MainForm.PackingFinished += (o, exitCode) =>
                        {
                            // Multiple messages can try to write to the same spon in the Console, hence lock
                            lock (syncObj)
                            {
                                if (exitCode == 0)
                                {
                                    // Write success message
                                    Console.SetCursorPosition(firstMessageCursorPos.left, firstMessageCursorPos.top);
                                    Console.WriteLine($"Packing... {maxValue} / {maxValue} done [100.00%]");
                                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                                    Console.WriteLine($"Packing complete!");
                                    Console.ResetColor();
                                    Console.WriteLine($"Packed app location: `{Path.GetFullPath(destinationPath)}`");
                                }
                                // Show error message if return code is abnormal
                                else
                                {
                                    Console.ForegroundColor = ConsoleColor.DarkRed;
                                    Console.WriteLine($"Packing finished with an error: {MainForm.GetErrorMessage(exitCode)}");
                                    Console.ResetColor();
                                }

                                // Release Sleep() loop
                                packingFinished = true;
                            }
                        };
                    }
                    // If --quiet then subscribe only to PackingFinished event to release Sleep() loop and display possible errors
                    else
                    {
                        MainForm.PackingFinished += (o, exitCode) =>
                        {
                            // Show error message if return code is abnormal
                            if (exitCode != 0)
                            {
                                Console.WriteLine();
                                Console.ForegroundColor = ConsoleColor.DarkRed;
                                Console.WriteLine($"Packing finished with an error: {MainForm.GetErrorMessage(exitCode)}");
                                Console.ResetColor();
                            }

                            // Release Sleep() loop
                            packingFinished = true;
                        };
                    }

                    // Initiate packing process
                    MainForm.StartPacking(sourceAppFolder, mainExePath, destinationPath, 
                                          customIconPath:customIconPath, 
                                          selfRepackable:selfRepackable, 
                                          launchArguments: launchArguments,
                                          customFileDescription: fileDescription,
                                          openUnpackDir:openUnpackedDir, 
                                          unpackDirectory:unpackDir,
                                          noGUI: true,
                                          passHash: Password.GetPasswordHashString(password),
                                          isWindowlessUnpacker: isWindowlessUnpacker);

                    // Keep the process alive until packing process finishes in order to receive progress messages and to clean up temp files
                    while (!packingFinished)
                    {
                        Thread.Sleep(200);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw e;
            }
            // When Appacker exits, send {Enter} key press to the parent command prompt (if not in --quiet mode)
            finally
            {
                if (!quietPacking)
                {
                    Console.WriteLine();
                    Console.Write(Environment.CurrentDirectory + ">");
                }
            }
        }

        // Checks for null and paths existance. Prints error messages
        // Ugly unreadable method, but it works and makes console ui pretty
        private static bool ValidateArguments(string sourceAppFolder, string mainExePath, string destinationPath, string customIconPath, bool isQuiet = false)
        {
            bool newLinePrinted = false;

            bool valid = true;
            bool sourcePathIsOk = true;

            ConsoleColor errorColor = ConsoleColor.DarkRed;
            ConsoleColor errorHighlightColor = ConsoleColor.Red;
            ConsoleColor warningColor = ConsoleColor.DarkYellow;

            if (sourceAppFolder == null)
            {
                if(!newLinePrinted)
                {
                    Console.WriteLine();
                    newLinePrinted = true;
                }
                Console.ForegroundColor = errorColor;
                Console.WriteLine("-s argument is missing");
                valid = false;
            }
            else if (!Directory.Exists(sourceAppFolder))
            {
                if (!newLinePrinted)
                {
                    Console.WriteLine();
                    newLinePrinted = true;
                }
                Console.ForegroundColor = errorColor;
                Console.Write("Invalid path to the source app folder: \"");
                Console.ForegroundColor = errorHighlightColor;
                Console.Write(sourceAppFolder);
                Console.ForegroundColor = errorColor;
                Console.WriteLine("\"");
                valid = false;
                sourcePathIsOk = false;
            }
            if (mainExePath == null)
            {
                if (!newLinePrinted)
                {
                    Console.WriteLine();
                    newLinePrinted = true;
                }
                Console.ForegroundColor = errorColor;
                Console.WriteLine("-e argument is missing");
                valid = false;
            }
            // If path to the source folder is wrong then there's no reason to check local path to the exe, hence `sourcePathIsOk`
            else if (sourcePathIsOk && !File.Exists(Path.Combine(sourceAppFolder, mainExePath)))
            {
                if (!newLinePrinted)
                {
                    Console.WriteLine();
                    newLinePrinted = true;
                }
                Console.ForegroundColor = errorColor;
                Console.Write($"Invalid local path to the main executable: \"");
                Console.ForegroundColor = errorHighlightColor;
                Console.Write(mainExePath);
                Console.ForegroundColor = errorColor;
                Console.WriteLine("\"");
                valid = false;
            }
            // Suppress warning for quiet
            if (!isQuiet && destinationPath != null && !destinationPath.EndsWith(".exe"))
            {
                if (!newLinePrinted)
                {
                    Console.WriteLine();
                    newLinePrinted = true;
                }
                Console.ForegroundColor = warningColor;
                Console.WriteLine("Warning: destination path should end with .exe");
                var dir = Path.GetDirectoryName(destinationPath);
                Console.WriteLine($"File '{Path.GetFileName(destinationPath)}' will be created at '{(string.IsNullOrWhiteSpace(dir) ? Environment.CurrentDirectory : Path.GetFullPath(dir))}'");
                // Don't mark as an error
            }
            if (customIconPath != null && !File.Exists(customIconPath))
            {
                if (!newLinePrinted)
                {
                    Console.WriteLine();
                    newLinePrinted = true;
                }
                Console.ForegroundColor = errorColor;
                Console.Write($"Invalid icon path: file does not exist: \"");
                Console.ForegroundColor = errorHighlightColor;
                Console.Write(customIconPath);
                Console.ForegroundColor = errorColor;
                Console.WriteLine("\"");
                valid = false;
            }

            if (!isQuiet && !newLinePrinted)
            {
                Console.WriteLine();
                newLinePrinted = true;
            }

            Console.ResetColor();
            return valid;
        }

        static void ShowHelp(OptionSet p, string exeName)
        {
            Console.WriteLine();
            Console.WriteLine($"v{System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString(3)}");
            Console.WriteLine("Tool for packing multi-file applications into a single .exe file");
            Console.WriteLine(USAGE, exeName);
            Console.WriteLine();
            Console.WriteLine("Options:");
            p.WriteOptionDescriptions(Console.Out);
        }
    }
}
