using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using XDMessaging;

namespace Packer
{
    class Program
    {
        private const string USAGE = "packer.exe <path to unpacker.exe> <path to packed app file> <local path to main exe> <path to folder for packing> [if app should be repackable: True|False] [main exe launch arguments]";
        private const int WAIT_FOR_FILE_ACCESS_TIMEOUT = 5000; // ms

        //XDMessaging broadcaster to report packing progress
        private static IXDBroadcaster broadcaster = null;

        // Timer that measures for how long packer is running
        private static readonly Stopwatch timer = new Stopwatch();
        private const int SPLASH_POPUP_DELAY = 1000; // ms

        //string unpackerExePath, string pathToPackedApp, string localPathToMainExe, string pathToFolderWithApp
        static int Main(string[] args)
        {
#if DEBUG
            Console.WriteLine("Packer.exe is running in Debug");
            Console.WriteLine("Attach to process now and press Enter...");
            Console.ReadLine();
            Console.WriteLine("Resuming");
#endif

            string unpackerExePath, pathToPackedApp, localPathToMainExe, pathToFolderWithApp, launchArguments, passHash;
            bool isSelfRepackable, isRepacking, openUnpackedDir; 
            bool isNoGui = false;   // determines the type of XDMessaging mode
            int unpackDirectory;    // Temp = 0, Desktop = 1, SameAsPackedExe = 2 or AskAtLaunch = 3

            try
            {
                #region == Arguments check and assignment ==
                if (args.Length < 4)
                {
                    Console.WriteLine("Arguments are missing. Usage:");
                    Console.WriteLine(USAGE);
                    return 1;
                }

                // UPD: i should have used arguments parser library... Mistakes were made and i don't want to refactor them now .__.
                // UPD2: what a shitty code. I hate myself.
                // UPD3: yeah, i hate you too, past me
                unpackerExePath = args[0];
                pathToPackedApp = args[1];
                localPathToMainExe = args[2];
                pathToFolderWithApp = args[3];
                launchArguments = "";
                if (args.Length > 4)
                    launchArguments = args[4];
                isSelfRepackable = false;
                if (args.Length > 5)
                    bool.TryParse(args[5], out isSelfRepackable);
                if (args.Length > 6)
                    bool.TryParse(args[6], out isNoGui);
                openUnpackedDir = false;
                if (args.Length > 7)
                    bool.TryParse(args[7], out openUnpackedDir);
                unpackDirectory = 0;
                if (args.Length > 8)
                {
                    int.TryParse(args[8], out unpackDirectory);
                    if (unpackDirectory < 0 || unpackDirectory > 3)
                        unpackDirectory = 0;
                }
                isRepacking = false;
                passHash = string.Empty;
                if (args.Length > 9)
                {
                    if (args[9] == "-repack" || args[9] == "repack")
                        isRepacking = true;
                    else
                    {
                        passHash = args[9];
                        if (args.Length > 10 && (args[10] == "-repack" || args[10] == "repack"))
                            isRepacking = true;
                    }
                }
                

                // Create XDMessagingClient broadcaster to report progress
                // Creating it here so the broadcaster is initialized before any possible return => finally
                XDMessagingClient client = new XDMessagingClient();
                // For command line launch use Compatibility mode, for GUI use HighPerformanceUI
                broadcaster = client.Broadcasters.GetBroadcasterForMode(isNoGui ? XDTransportMode.Compatibility : XDTransportMode.HighPerformanceUI);

                if (!File.Exists(unpackerExePath))
                {
                    Console.WriteLine("Unpacker.exe is missing.");
                    return 2;
                }

                if (!Directory.Exists(pathToFolderWithApp))
                {
                    Console.WriteLine("Specified directory with application does not exist.");
                    return 3;
                }

                if (!File.Exists(Path.Combine(pathToFolderWithApp, localPathToMainExe)))
                {
                    Console.WriteLine("Main executable does not exist in app directory.");
                    return 4;
                }

                // Check if the provided path where we gonna save packed exe is valid
                FileStream testFile = null;
                try
                {
                    testFile = File.Create(pathToPackedApp + ".temp", 1, FileOptions.DeleteOnClose);
                }
                catch (Exception)
                {
                    Console.WriteLine("Invalid path to packed executable.");
                    return 5;
                }
                finally
                {
                    testFile?.Close();
                }
                #endregion
                
                // Get all files in the application folder (incl. sub-folders)
                List<string> filesToPack = GetFilesRecursively(pathToFolderWithApp);

                // Transform absolute paths into relative ones
                for (int i = 0; i < filesToPack.Count; i++)
                    filesToPack[i] = filesToPack[i].Replace(pathToFolderWithApp, string.Empty).TrimStart('\\');

                // If it's self-repacking process, then we should wait until packed app frees its .exe file
                if (isRepacking)
                {
                    if (WaitForFileAccess(pathToPackedApp, WAIT_FOR_FILE_ACCESS_TIMEOUT) == false)
                    {
                        Console.WriteLine($"Can't access file {pathToPackedApp}");
                        return 6;
                    }
                }

                timer.Start();

                // Do the packing
                try
                {
                    PackApp(unpackerExePath, pathToPackedApp, pathToFolderWithApp, localPathToMainExe, filesToPack, isSelfRepackable, launchArguments, isRepacking, openUnpackedDir, unpackDirectory, passHash);
                }
                catch (UnauthorizedAccessException)
                {
                    Console.WriteLine($"Unauthorized access to {pathToPackedApp}");
                    return 6;
                }
                catch (Exception)
                {
                    Console.WriteLine($"Oops! Something went wrong");
                    return -1;
                }

                // If it was repack call, then packer.exe is the one who should remove the temp dir with the app after packing
                // Problem: packer.exe can't delete itself
                // Solution: launch just repacked app and tell it to kill this packer.exe
                if (isRepacking)
                {
                    if (Directory.Exists(pathToFolderWithApp))
                        Directory.Delete(pathToFolderWithApp, true);

                    Process.Start(pathToPackedApp, $@"-killme ""{System.Reflection.Assembly.GetEntryAssembly().Location}""");
                }

                return 0;
            }
            finally
            {
                broadcaster.SendToChannel("AppackerProgress", "Done");
            }
        }

        /// <summary>
        /// Packs given files into a single executable file
        /// </summary>
        /// <param name="unpackerExePath">Path to Unpacker.exe</param>
        /// <param name="pathToPackedApp">Path where packed .exe will be created</param>
        /// <param name="pathToFolderWithApp">Path to the directory that contains application files that will be packed</param>
        /// <param name="localPathToMainExe">Relative path to the executable file that will be launched when the packed app is launched</param>
        /// <param name="filesToPack">List of relative paths to all files of the app that is being packed</param>
        /// <param name="unpackDirectory">Enum as int, 0 to 3, indicates whereto unpack files at launch</param>
        /// <param name="openUnpackedDir">Whether or not to open folder with unpacked files after unpacking</param>
        /// <param name="launchArguments">Arguments to pass to target exe with every launch</param>
        /// <param name="passHash">Hash of a password for lauching packed app</param>
        private static void PackApp(string unpackerExePath, string pathToPackedApp, string pathToFolderWithApp, string localPathToMainExe, List<string> filesToPack, bool isSelfRepackable, string launchArguments, bool isRepacking, bool openUnpackedDir, int unpackDirectory, string passHash)
        {
            using (var packedExe = new BinaryWriter(File.Open(pathToPackedApp, FileMode.Create, FileAccess.Write)))
            {
                // Write wrapper app (unpacker.exe) 
                // and the key-word mark that will help separate wrapper app bytes from data bytes
                packedExe.Write(File.ReadAllBytes(unpackerExePath));    // byte[]
                packedExe.Write(Encoding.UTF8.GetBytes("<SerGreen>"));  // byte[]

                // Write password flag and password hash (if not empty)
                bool hasPassword = !string.IsNullOrEmpty(passHash);
                packedExe.Write(hasPassword);
                if (hasPassword)
                    packedExe.Write(ConvertHexStringToByteArray(passHash));

                // Write self-repackable flag
                packedExe.Write(isSelfRepackable);                      // bool
                // If self-repackable, then write packer.exe
                if (isSelfRepackable)
                {
                    byte[] packerData = File.ReadAllBytes(System.Reflection.Assembly.GetEntryAssembly().Location);
                    packedExe.Write(packerData.Length);                 // int
                    packedExe.Write(packerData);                        // byte[]
                }

                // If ProgressBarSplash.exe is present in the same forder, then add it to the package
                string splashPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ProgressBarSplash.exe");
                bool splashExeExists = File.Exists(splashPath);
                packedExe.Write(splashExeExists);                       // bool
                if (splashExeExists)
                {
                    byte[] splashData = File.ReadAllBytes(splashPath);
                    packedExe.Write(splashData.Length);                 // int
                    packedExe.Write(splashData);                        // byte[]
                }

                // Write open unpacking directory flag
                packedExe.Write(openUnpackedDir);                       // bool

                // Write unpacking directory type
                packedExe.Write(unpackDirectory);                       // int

                // Write relative path to the main executable of the packed app
                packedExe.Write(localPathToMainExe);                    // string

                // Write launch arguments for main executable
                packedExe.Write(launchArguments);                       // string

                Process splashProgressBarProc = null;

                // Append all packed files to the end of the wrapper app .exe file
                for (int i=0; i<filesToPack.Count; i++)
                {
                    // If repacking is going on for too long, show the splash progress bar                    
                    if (isRepacking && splashProgressBarProc == null && timer.ElapsedMilliseconds > SPLASH_POPUP_DELAY)
                    {
                        timer.Stop();
                        if (splashExeExists)
                            splashProgressBarProc = Process.Start(splashPath, "-packing");
                    }

                    // Report progress to Appacker
                    broadcaster.SendToChannel("AppackerProgress", $"{i} {filesToPack.Count}");

                    byte[] data = File.ReadAllBytes(Path.Combine(pathToFolderWithApp, filesToPack[i]));
                    packedExe.Write(filesToPack[i]);                    // string
                    packedExe.Write(data.Length);                       // int
                    packedExe.Write(data);                              // byte[]
                }

                splashProgressBarProc?.Kill();
            }
        }

        /// <summary>
        /// Get all files paths from the directory and its subdirectories
        /// </summary>
        /// <param name="dirPath">Path to the directory</param>
        /// <returns>List of absolute paths to all files within given directory and its subdirectories</returns>
        private static List<string> GetFilesRecursively(string dirPath)
        {
            List<string> files = new List<string>();
            files.AddRange(Directory.GetFiles(dirPath));

            foreach (string subDir in Directory.GetDirectories(dirPath))
                files.AddRange(GetFilesRecursively(subDir));

            return files;
        }
        
        /// <summary>
        /// Awaits for file access
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

        private static byte[] ConvertHexStringToByteArray (string hexString)
        {
            if (hexString.Length != 64)
            {
                throw new ArgumentException(string.Format("Password hash should be exactly 32 bytes long (64 hex characters): {0}", hexString));
            }

            byte[] data = new byte[hexString.Length / 2];
            for (int index = 0; index < data.Length; index++)
            {
                string byteValue = hexString.Substring(index * 2, 2);
                data[index] = Convert.ToByte(byteValue, 16);
            }

            return data;
        }
    }
}
