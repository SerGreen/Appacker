using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using XDMessaging;
using System.Windows.Forms;

namespace Unpacker
{
    class Program
    {
        // WinAPI methods used to hide console window when stdout is not redirected
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();
        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        const int SW_HIDE = 0;
        const int SW_SHOW = 5;

        private const int WAIT_FOR_FILE_ACCESS_TIMEOUT = 5000; // ms
        // Path to directory where target app gets extracted
        private static string tempDir = null;
        // Where create temp directory where target app gets extracted. 0 = temp, 1 = desktop, 2 = where this exe is, 3 = ask user
        private static int unpackingDirectoryType = 0;
        private static string unpackingDirectoryPath = null;
        // Path to directory where Appacker tools get extracted
        private static string repackerTempDir = null;
        // A flag that indicated whether or not we need to repack target app after it quits
        private static bool isSelfRepackable = false;
        // A flag that indicated whether or not to open folder with unpacked files in Explorer
        private static bool openUnpackedDir = false;
        // Local path to the exe file inside target app dir, that needs to be launched
        private static string pathToMainExe = null;
        // Arguments to pass to target app
        private static string launchArguments = null;
        // A flag that indicates whether or not the splash screen tool is present inside the package
        private static bool isProgressBarSplashExePresent;
        // A time stamp that will help to determine whether or not any file of target app was changed (i.e. the need of repacking)
        private static DateTime unpackingDoneTimestamp;
        // Same purpose, helps to detect deletion of files from target app
        private static int targetAppFilesCount = 0;

        //XDMessaging broadcaster to report unpacking progress
        private static IXDBroadcaster broadcaster = null;

        // Timer that measures for how long packer is running
        private static readonly Stopwatch timer = new Stopwatch();
        private const int SPLASH_POPUP_DELAY = 1000; // ms

        [STAThread]
        static void Main(string[] args)
        {
#if DEBUG
            Console.WriteLine("Unpacker is running in Debug");
            Console.WriteLine("Attach to process now and press Enter...");
            Console.ReadLine();
            Console.WriteLine("Resuming");
#endif

            // Hide console window if stdout is not redirected
            // If it is => keep window and later redirect I/O of the unpacked app too
            bool stdoutRedirected = Console.IsOutputRedirected;
            if (!stdoutRedirected)
#if DEBUG
                Console.WriteLine($"Output redirected: {stdoutRedirected}");
                Console.WriteLine("I'm hidden");
#else
                ShowWindow(GetConsoleWindow(), SW_HIDE);
#endif

            // After self-repacking, packer.exe can't delete itself from the temp dir, so it calls repacked app to kill it
            // Command is "unpacker.exe -killme <path to packer.exe>"
            // The whole folder where packer.exe is gets deleted (cuz there should also be temp unpacker.exe from repacking process)
            if (args.Length > 0 &&
            (args[0] == "-killme" || args[0] == "killme"))
            {
                if (args.Length > 1)
                    if (WaitForFileAccess(args[1], WAIT_FOR_FILE_ACCESS_TIMEOUT) == true)
                        DeleteDirectory(Path.GetDirectoryName(args[1]));

                // Don't unpack anything. Just close the app now
                return;
            }

                        
            // Subscribe for exit event to delete tempDir
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(Unpacker_ProcessExit);
            
            // Open self exe file as byte stream and unpack data that is appended to the end of exe
            string selfExePath = System.Reflection.Assembly.GetEntryAssembly().Location;
            using (var me = new BinaryReader(File.OpenRead(selfExePath)))
            {
                // Find the end of the actual unpacker.exe
                byte[] pattern = Encoding.UTF8.GetBytes("<SerGreen>");
                long pos = FindPatternPosition(me.BaseStream, pattern);
                me.BaseStream.Seek(pos + pattern.Length, SeekOrigin.Begin);

                // Create temp directory to store tools
                while (repackerTempDir == null || Directory.Exists(repackerTempDir))
                    repackerTempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                Directory.CreateDirectory(repackerTempDir);

                // Start extracting appended data
                isSelfRepackable = me.ReadBoolean();
                if(isSelfRepackable)
                {
                    // For repacking we also need the unpacker.exe itself
                    // So return to the beginning of the file and save the unpacker.exe part
                    me.BaseStream.Seek(0, SeekOrigin.Begin);
                    File.WriteAllBytes(Path.Combine(repackerTempDir, "unpacker.exe"), me.ReadBytes((int)pos));
                    // Skip <SerGreen> keyword and 1 byte of isSelfRepackable flag
                    me.ReadBytes(pattern.Length + 1);
                    // Save the packer.exe
                    int packerDataLength = me.ReadInt32();
                    File.WriteAllBytes(Path.Combine(repackerTempDir, "packer.exe"), me.ReadBytes(packerDataLength));
                }

                // Extract splash screen tool if present
                isProgressBarSplashExePresent = me.ReadBoolean();
                if(isProgressBarSplashExePresent)
                {
                    int splashDataLength = me.ReadInt32();
                    File.WriteAllBytes(Path.Combine(repackerTempDir, "ProgressBarSplash.exe"), me.ReadBytes(splashDataLength));
                }

                // Unpack directory loading
                openUnpackedDir = me.ReadBoolean();
                unpackingDirectoryType = me.ReadInt32();

                // Decode actual path
                // Desktop
                if (unpackingDirectoryType == 1)
                    unpackingDirectoryPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                // Directory where this exe is located
                else if (unpackingDirectoryType == 2)
                    unpackingDirectoryPath = AppDomain.CurrentDomain.BaseDirectory;
                // Ask user for a folder
                else if (unpackingDirectoryType == 3)
                {
                    FolderBrowserDialog fbd = new FolderBrowserDialog { Description = "Select a folder in which to extract files" };
                    if (fbd.ShowDialog() == DialogResult.OK)
                        unpackingDirectoryPath = fbd.SelectedPath;
                    else
                        unpackingDirectoryPath = Path.GetTempPath();
                }
                // Default is %TEMP%
                else
                    unpackingDirectoryPath = Path.GetTempPath();

                // Create temp directory to store unpacked files
                while (tempDir == null || Directory.Exists(tempDir))
                    tempDir = Path.Combine(unpackingDirectoryPath, Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetEntryAssembly().Location) + "_" + Guid.NewGuid().ToString());
                Directory.CreateDirectory(tempDir);

                // Load relative path to the main exe
                pathToMainExe = me.ReadString();

                // Load launch arguments for main exe
                launchArguments = me.ReadString();

                timer.Start();
                Process splashProgressBarProc = null;

                // Keep extracting files until the end of the stream
                while (me.BaseStream.Position < me.BaseStream.Length)
                {
                    // If unpacking process takes too long, display splash screen with progress bar
                    if(isProgressBarSplashExePresent && broadcaster == null && timer.ElapsedMilliseconds > SPLASH_POPUP_DELAY)
                    {
                        // Create XDMessagingClient broadcaster to report progress
                        XDMessagingClient client = new XDMessagingClient();
                        broadcaster = client.Broadcasters.GetBroadcasterForMode(XDTransportMode.HighPerformanceUI);

                        splashProgressBarProc = Process.Start(Path.Combine(repackerTempDir, "ProgressBarSplash.exe"), "-unpacking");
                        timer.Stop();
                    }

                    // Report progress to the Splash screen with progress bar
                    broadcaster?.SendToChannel("AppackerProgress", $"{me.BaseStream.Position} {me.BaseStream.Length}");

                    targetAppFilesCount++;
                    string path = me.ReadString();
                    int length = me.ReadInt32();
                    byte[] data = me.ReadBytes(length);

                    // Create subdirectory if necessary
                    string dir = Path.GetDirectoryName(path);
                    if (!string.IsNullOrWhiteSpace(dir) && !Directory.Exists(Path.Combine(tempDir, dir)))
                        Directory.CreateDirectory(Path.Combine(tempDir, dir));

                    File.WriteAllBytes(Path.Combine(tempDir, path), data);
                }

                unpackingDoneTimestamp = DateTime.Now;
                splashProgressBarProc?.Kill();
            }

            // Open tempDir in Explorer if corresponding flag is set
            if(openUnpackedDir)
            {
                Process.Start(new ProcessStartInfo() { FileName = "explorer.exe",
                                                       Arguments = tempDir,
                                                       UseShellExecute = true,
                                                       Verb = "open" });
            }

            // Launch unpacked app
            ProcessStartInfo procInfo = new ProcessStartInfo()
            {
                FileName = Path.Combine(tempDir, pathToMainExe),
                Arguments = launchArguments + " " + string.Join(" ", args.Select(x => $"\"{x}\"")),
                WorkingDirectory = tempDir,
                UseShellExecute = !stdoutRedirected     // if stdout is redirected, then redirect i/o of the target app too
            };
            Process proc = Process.Start(procInfo);

            // Wait until app exits to delete its files from temp directory
            proc.WaitForExit();
        }

        // Exit event. Delete tempDir recursively or start repacker process
        private static void Unpacker_ProcessExit(object sender, EventArgs e)
        {
            // If it's a self-repackable app, then launch packer.exe from temp directory
            if (isSelfRepackable)
            {
                // Repack only if there're actually changed files
                if (AnyFileChanged(tempDir))
                {
                    // Launch packer.exe with following arguments:
                    // 1. Path to unpacker.exe (inside repackTempDir)
                    // 2. Path where to save the packed app (path of the current exe, it will be replaced)
                    // 3. Relative path to main executable inside app directory
                    // 4. Path to the app directory (tempDir)
                    // 5. Self-repackable flag = True
                    // 6. Open unpacking temp folder flag
                    // 7. Unpacking directory type
                    // 8. -repack flag = mark, that this is the repacking process, which will result in deletion of unpacked temp folder after repacking
                    ProcessStartInfo repackProcInfo = new ProcessStartInfo(Path.Combine(repackerTempDir, "packer.exe"));
                    repackProcInfo.Arguments = $@"""{Path.Combine(repackerTempDir, "unpacker.exe")}"" ""{System.Reflection.Assembly.GetEntryAssembly().Location}"" ""{pathToMainExe}"" ""{tempDir}"" ""{launchArguments.Replace("\"", "\\\"")}"" True True {openUnpackedDir} {unpackingDirectoryType} -repack";
#if (!DEBUG)
                repackProcInfo.CreateNoWindow = true;
                repackProcInfo.WindowStyle = ProcessWindowStyle.Hidden;
#endif
                    Process.Start(repackProcInfo);
                }
                // If no file was changed, then just delete temp folders and that's it
                else
                {
                    DeleteDirectory(tempDir);
                    DeleteDirectory(repackerTempDir);
                }
            }
            // If it's not a repackable app, then delete temp files immediately
            else
            {
                DeleteDirectory(tempDir);
                DeleteDirectory(repackerTempDir);
            }

            // Make window appear again, otherwise when launched from command prompt there will be left hidden cmd
            ShowWindow(GetConsoleWindow(), SW_SHOW);
        }

        private static void DeleteDirectory(string dirPath)
        {
            if (dirPath != null && Directory.Exists(dirPath))
                Directory.Delete(dirPath, true);
        }

        // Checks if there's at least one file that has been altered, created or deleted since the finish of unpacking process
        private static bool AnyFileChanged(string pathToAppDir)
        {
            var allFiles = GetAllFilesInsideDirectory(new DirectoryInfo(pathToAppDir));
            if (allFiles.Any(x => x.LastWriteTime.Subtract(unpackingDoneTimestamp).Milliseconds > 200 
                               || x.CreationTime.Subtract(unpackingDoneTimestamp).Milliseconds > 200))
                return true;
            else if (allFiles.Count() < targetAppFilesCount)
                return true;

            return false;
        }
        private static IEnumerable<FileInfo> GetAllFilesInsideDirectory(DirectoryInfo dirInfo)
        {
            foreach (FileInfo file in dirInfo.GetFiles())
                yield return file;

            foreach (DirectoryInfo dir in dirInfo.GetDirectories())
                foreach (FileInfo file in GetAllFilesInsideDirectory(dir))
                    yield return file;
        }

#region == Find pattern position in stream methods ==
        // Solution from here https://stackoverflow.com/questions/1471975
        /// <summary>
        /// Search for the first occurance of a byte-sequence pattern in stream
        /// </summary>
        /// <param name="stream">Stream to search in</param>
        /// <param name="byteSequence">Pattern to search for</param>
        /// <returns>Index of sequence start</returns>
        public static long FindPatternPosition(Stream stream, byte[] byteSequence)
        {
            if (byteSequence.Length > stream.Length)
                return -1;

            byte[] buffer = new byte[byteSequence.Length];

            BufferedStream bufStream = new BufferedStream(stream, byteSequence.Length);
            int i;
            while ((i = bufStream.Read(buffer, 0, byteSequence.Length)) == byteSequence.Length)
            {
                if (byteSequence.SequenceEqual(buffer))
                    return bufStream.Position - byteSequence.Length;
                else
                    bufStream.Position -= byteSequence.Length - PadLeftSequence(buffer, byteSequence);
            }

            return -1;
        }

        private static int PadLeftSequence(byte[] bytes, byte[] seqBytes)
        {
            int i = 1;
            while (i < bytes.Length)
            {
                int n = bytes.Length - i;
                byte[] aux1 = new byte[n];
                byte[] aux2 = new byte[n];
                Array.Copy(bytes, i, aux1, 0, n);
                Array.Copy(seqBytes, aux2, n);
                if (aux1.SequenceEqual(aux2))
                    return i;
                i++;
            }
            return i;
        }
#endregion

#region == Wait for the file access ==
        /// <summary>
        /// Awaits for the file access
        /// </summary>
        /// <param name="timeout">Maximum amount of time to wait in milliseconds</param>
        /// <returns>True if access successfully gained; False if timed out</returns>
        private static bool WaitForFileAccess(string filePath, int timeout = 0)
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();
            while (!IsFileReady(filePath))
            {
                if (timeout > 0 && timer.ElapsedMilliseconds > timeout)
                    return false;

                System.Threading.Thread.Sleep(200);
            }
            timer.Stop();
            return true;
        }

        /// <summary>
        /// Detects if you can exclusively access the file
        /// </summary>
        /// <param name="sFilename">Path to file</param>
        /// <returns>True if file can be exclusively accessed, False otherwise</returns>
        private static bool IsFileReady(String sFilename)
        {
            // If the file can be opened for exclusive access it means that the file
            // is no longer locked by another process.
            try
            {
                using (FileStream inputStream = File.Open(sFilename, FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    if (inputStream.Length > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                }
            }
            catch (Exception)
            {
                return false;
            }
        }
#endregion
    }
}
