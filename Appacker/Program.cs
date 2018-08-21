using NDesk.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using XDMessaging;

namespace Appacker
{
    static class Program
    {
        [DllImport("kernel32.dll")]
        static extern bool AttachConsole(int dwProcessId);
        private const int ATTACH_PARENT_PROCESS = -1;

        private const string TRY_HELP = "Try `{0} --help' for more information.";
        private const string USAGE = @"Usage: {0} [-r] [-q] <-s ""source_folder""> <-e ""main_exe""> <-d ""save_location""> [-i ""icon_path""]";

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
            string sourceAppFolder = null, mainExePath = null, destinationPath = null, customIconPath = null;
            bool selfRepackable = false, quietPacking = false;
            bool showHelp = false;

            var argsParser = new OptionSet() {
                    { "s|src|source-app-folder=",
                        "path to the folder containing all the files of the target application",
                        s => sourceAppFolder = s.TrimEnd('\\', '/') },
                    { "e|exe|main-exe=",
                        "local path to the main executable inside the target app folder",
                        e => mainExePath = e.TrimStart('\\', '/') },
                    { "d|dst|destination-path=",
                        "location where packed app will be saved",
                        d => destinationPath = d },
                    { "i|ico|icon=",
                        "path to the custom icon",
                        c => customIconPath = c },
                    { "r|self-repack",
                        "makes application self-repackable",
                        r => selfRepackable = r != null },
                    { "q|quiet|silent",
                        "no packing progress messages will be shown",
                        q => quietPacking = q != null },
                    { "h|help|?",
                        "show this message and exit",
                       v => showHelp = v != null }
                };

            List<string> unknownArgs;
            try
            {
                unknownArgs = argsParser.Parse(args);
            }
            catch (OptionException e)
            {
                Console.WriteLine("Appacker: " + e.Message);
                Console.WriteLine(TRY_HELP, args[0]);
                return;
            }

            if (showHelp)
            {
                ShowHelp(argsParser, args[0]);
                return;
            }
            // Validate arguments and show error messages if required
            else
            {
                #region Arguments validation
                Console.WriteLine();
                bool error = false;
                if (sourceAppFolder == null)
                {
                    Console.WriteLine("-s argument is missing");
                    error = true;
                }
                else if (!Directory.Exists(sourceAppFolder))
                {
                    Console.WriteLine("Invalid path to the source app folder");
                    error = true;
                }
                if (mainExePath == null)
                {
                    Console.WriteLine("-e argument is missing");
                    error = true;
                }
                else if (!File.Exists(Path.Combine(sourceAppFolder, mainExePath)))
                {
                    Console.WriteLine("Invalid local path to the main executable");
                    error = true;
                }
                if (destinationPath == null)
                {
                    Console.WriteLine("-d argument is missing");
                    error = true;
                }
                if (customIconPath != null && !File.Exists(customIconPath))
                {
                    Console.WriteLine("Invalid icon path: file does not exist");
                    error = true;
                }
                if (error)
                {
                    Console.WriteLine(USAGE, args[0]);
                    Console.WriteLine(TRY_HELP, args[0]);
                    return;
                }
                #endregion

                // == PACKING ==
                // Subscribe to progress update events
                if (!quietPacking)
                {
                    XDMessagingClient client = new XDMessagingClient();
                    IXDListener listener = client.Listeners.GetListenerForMode(XDTransportMode.HighPerformanceUI);
                    listener.RegisterChannel("AppackerProgress");

                    MainForm.PackingProgressUpdate += (o, progress) =>
                    {
                        Console.WriteLine($"Packing... {progress.currentValue} / {progress.maxValue} done [{(float)progress.currentValue / progress.maxValue:F2}%]");
                    };
                    MainForm.PackingFinished += (o, exitCode) =>
                    {
                        // Show error message if return code is abnormal
                        if (exitCode == 0)
                            Console.WriteLine($"Packing complete!\n Packed app location: `{destinationPath}`");
                        else
                            Console.WriteLine($"Packing finished with an error:\n {MainForm.GetErrorMessage(exitCode)}");
                    };
                }

                MainForm.StartPacking(sourceAppFolder, mainExePath, destinationPath, customIconPath, selfRepackable);
            }
        }

        static void ShowHelp(OptionSet p, string exeName)
        {
            Console.WriteLine();
            Console.WriteLine("Tool for packing multi-file applications into single .exe file");
            Console.WriteLine(USAGE, exeName);
            Console.WriteLine();
            Console.WriteLine("Options:");
            p.WriteOptionDescriptions(Console.Out);
        }
    }
}
