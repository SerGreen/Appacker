using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Packer
{
    class Program
    {
        private const string USAGE = "packer.exe <path to unpacker.exe> <path to packed app file> <local path to main exe> <path to folder for packing>";

        //string unpackerExePath, string pathToPackedApp, string localPathToMainExe, string pathToFolderWithApp
        static int Main(string[] args)
        {
#if DEBUG
            Console.WriteLine("Running in Debug");
            Console.WriteLine("Attach to process now and press Enter...");
            Console.ReadLine();
            Console.WriteLine("Resuming");
#endif

            #region == Arguments check and assignment ==
            if (args.Length < 4)
            {
                Console.WriteLine("Arguments are missing. Usage:");
                Console.WriteLine(USAGE);
                return 1;
            }

            string unpackerExePath = args[0];
            string pathToPackedApp = args[1];
            string localPathToMainExe = args[2];
            string pathToFolderWithApp = args[3];
            
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

            // Get all files in the application folder (incl. sub-folders)
            List<string> filesToPack = GetFilesRecursively(pathToFolderWithApp);
            
            // Transform absolute paths into relative ones
            for (int i = 0; i < filesToPack.Count; i++)
                filesToPack[i] = filesToPack[i].Replace(pathToFolderWithApp, string.Empty).TrimStart('\\');

            PackApp(unpackerExePath, pathToPackedApp, pathToFolderWithApp, localPathToMainExe, filesToPack);

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
        private static void PackApp(string unpackerExePath, string pathToPackedApp, string pathToFolderWithApp, string localPathToMainExe, List<string> filesToPack)
        {
            using (var packedExe = new BinaryWriter(File.Open(pathToPackedApp, FileMode.Create, FileAccess.Write)))
            {
                // Write wrapper app (unpacker.exe) 
                // and the key-word mark that will help to separate wrapper app bytes from data bytes
                packedExe.Write(File.ReadAllBytes(unpackerExePath));      // byte[]
                packedExe.Write(Encoding.UTF8.GetBytes("<SerGreen>"));    // byte[]

                // Write relative path to the main executable of the packed app
                packedExe.Write(localPathToMainExe);                    // string

                // Write all packed files to the end of wrapper app .exe file
                foreach (string filePath in filesToPack)
                {
                    byte[] data = File.ReadAllBytes(Path.Combine(pathToFolderWithApp, filePath));
                    packedExe.Write(filePath);                          // string
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
    }
}
