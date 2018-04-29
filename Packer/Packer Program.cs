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
        private const string USAGE = "packer.exe <path_to_packed_app> <local_path_to_main_exe> <path_to_folder_for_packing>";

        //string pathToPackedApp, string localPathToMainExe, string pathToFolderWithApp
        static void Main(string[] args)
        {
            #region == Arguments check and assignment ==
            if (args.Length < 3)
            {
                Console.WriteLine("Arguments are missing. Usage:");
                Console.WriteLine(USAGE);
                return;
            }

            string pathToPackedApp = args[0];
            string localPathToMainExe = args[1];
            string pathToFolderWithApp = args[2];
            
            if (!Directory.Exists(pathToFolderWithApp))
            {
                Console.WriteLine("Specified directory with application does not exist.");
                return;
            }

            if (!File.Exists(Path.Combine(pathToFolderWithApp, localPathToMainExe)))
            {
                Console.WriteLine("Main executable does not exist in app directory.");
                return;
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
                return;
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

            PackApp(pathToPackedApp, pathToFolderWithApp, localPathToMainExe, filesToPack);
        }

        /// <summary>
        /// Packs given files into a single executable file
        /// </summary>
        /// <param name="pathToPackedApp">Path where packed .exe will be created</param>
        /// <param name="pathToFolderWithApp">Path to the directory that contains application files that will be packed</param>
        /// <param name="localPathToMainExe">Relative path to the executable file that will be launched when the packed app is launched</param>
        /// <param name="filesToPack">List of relative paths to all files of the app that is being packed</param>
        private static void PackApp(string pathToPackedApp, string pathToFolderWithApp, string localPathToMainExe, List<string> filesToPack)
        {
            using (var packedExe = new BinaryWriter(File.Open(pathToPackedApp, FileMode.Create, FileAccess.Write)))
            {
                // Write wrapper app (unpacker.exe) 
                // and the key-word mark that will help to separate wrapper app bytes from data bytes
                packedExe.Write(Resource.unpacker);                     // byte[]
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
