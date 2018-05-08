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
        private const string USAGE = "packer.exe <path to unpacker.exe> <path to packed app file> <local path to main exe> <path to folder for packing> [if app should be repackable: True|False]";
        private const int WAIT_FOR_FILE_ACCESS_TIMEOUT = 5000; // ms

        //XDMessaging broadcaster to report packing progress
        private static IXDBroadcaster broadcaster = null;

        //string unpackerExePath, string pathToPackedApp, string localPathToMainExe, string pathToFolderWithApp
        static int Main(string[] args)
        {
#if DEBUG
            Console.WriteLine("Packer.exe is running in Debug");
            Console.WriteLine("Attach to process now and press Enter...");
            Console.ReadLine();
            Console.WriteLine("Resuming");
#endif

            string unpackerExePath, pathToPackedApp, localPathToMainExe, pathToFolderWithApp;
            bool isSelfRepackable, isRepacking;

            #region == Arguments check and assignment ==
            if (args.Length < 4)
            {
                Console.WriteLine("Arguments are missing. Usage:");
                Console.WriteLine(USAGE);
                return 1;
            }

            unpackerExePath = args[0];
            pathToPackedApp = args[1];
            localPathToMainExe = args[2];
            pathToFolderWithApp = args[3];
            isSelfRepackable = false;
            if (args.Length > 4)
                bool.TryParse(args[4], out isSelfRepackable);
            isRepacking = false;
            if (args.Length > 5 && (args[5] == "-repack" || args[5] == "repack"))
                isRepacking = true;
            
            if(!File.Exists(unpackerExePath))
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
            catch(Exception)
            {
                Console.WriteLine("Invalid path to packed executable.");
                return 5;
            }
            finally
            {
                testFile?.Close();
            }
            #endregion

            if (!isRepacking)
            {
                // Create XDMessagingClient broadcaster to report progress
                XDMessagingClient client = new XDMessagingClient();
                broadcaster = client.Broadcasters.GetBroadcasterForMode(XDTransportMode.HighPerformanceUI);
            }

            // Get all files in the application folder (incl. sub-folders)
            List<string> filesToPack = GetFilesRecursively(pathToFolderWithApp);
            
            // Transform absolute paths into relative ones
            for (int i = 0; i < filesToPack.Count; i++)
                filesToPack[i] = filesToPack[i].Replace(pathToFolderWithApp, string.Empty).TrimStart('\\');

            // If it's self-repacking process, then we should wait until packed app frees its .exe file
            if (isRepacking)
            {
                if(WaitForFileAccess(pathToPackedApp, WAIT_FOR_FILE_ACCESS_TIMEOUT) == false)
                {
                    Console.WriteLine($"Can't access file {pathToPackedApp}");
                    return 6;
                }
            }

            // Do the packing
            PackApp(unpackerExePath, pathToPackedApp, pathToFolderWithApp, localPathToMainExe, filesToPack, isSelfRepackable);

            // If it was repack call, then packer.exe is the one who should remove the temp dir with the app after packing
            // Problem: packer.exe can't delete itself
            // Solution: launch just repacked app and tell it to kill this packer.exe
            if(isRepacking)
            {
                if (Directory.Exists(pathToFolderWithApp))
                    Directory.Delete(pathToFolderWithApp, true);

                Process.Start(pathToPackedApp, $@"-killme ""{System.Reflection.Assembly.GetEntryAssembly().Location}""");
            }

            broadcaster?.SendToChannel("PackerProgress", "Done");
            return 0;
        }

        /// <summary>
        /// Packs given files into a single executable file
        /// </summary>
        /// <param name="unpackerExePath">Path to Unpacker.exe</param>
        /// <param name="pathToPackedApp">Path where packed .exe will be created</param>
        /// <param name="pathToFolderWithApp">Path to the directory that contains application files that will be packed</param>
        /// <param name="localPathToMainExe">Relative path to the executable file that will be launched when the packed app is launched</param>
        /// <param name="filesToPack">List of relative paths to all files of the app that is being packed</param>
        private static void PackApp(string unpackerExePath, string pathToPackedApp, string pathToFolderWithApp, string localPathToMainExe, List<string> filesToPack, bool isSelfRepackable)
        {
            using (var packedExe = new BinaryWriter(File.Open(pathToPackedApp, FileMode.Create, FileAccess.Write)))
            {
                // Write wrapper app (unpacker.exe) 
                // and the key-word mark that will help to separate wrapper app bytes from data bytes
                packedExe.Write(File.ReadAllBytes(unpackerExePath));    // byte[]
                packedExe.Write(Encoding.UTF8.GetBytes("<SerGreen>"));  // byte[]

                // Write self-repackable flag
                packedExe.Write(isSelfRepackable);                      // bool
                // If self-repackable, then write packer.exe
                if (isSelfRepackable)
                {
                    byte[] packerData = File.ReadAllBytes(System.Reflection.Assembly.GetEntryAssembly().Location);
                    packedExe.Write(packerData.Length);                 // int
                    packedExe.Write(packerData);                        // byte[]
                }

                // Write relative path to the main executable of the packed app
                packedExe.Write(localPathToMainExe);                    // string

                // Append all packed files to the end of the wrapper app .exe file
                for (int i=0; i<filesToPack.Count; i++)
                {
                    // Report progress to Appacker
                    broadcaster?.SendToChannel("PackerProgress", $"{i} {filesToPack.Count}");

                    byte[] data = File.ReadAllBytes(Path.Combine(pathToFolderWithApp, filesToPack[i]));
                    packedExe.Write(filesToPack[i]);                    // string
                    packedExe.Write(data.Length);                       // int
                    packedExe.Write(data);                              // byte[]
                }
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
    }
}
