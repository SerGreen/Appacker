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
        private const string USAGE = "Usage: \npacker.exe <path_to_packed_app> <local_path_to_main_exe> <path_to_folder_for_packing>";

        //string pathToPackedApp, string localPathToMainExe, string pathToFolderWithApp
        static void Main(string[] args)
        {
            if (args.Length < 3)
            {
                Console.WriteLine(USAGE);
                return;
            }

            string pathToPackedApp = args[0];
            string localPathToMainExe = args[1];
            string pathToFolderWithApp = args[2];

            if (pathToPackedApp == null ||
                localPathToMainExe == null ||
                pathToFolderWithApp == null)
            {
                Console.WriteLine(USAGE);
                return;
            }

            if(!Directory.Exists(pathToFolderWithApp))
            {
                Console.WriteLine("Specified directory with application does not exist.");
                return;
            }

            if (!File.Exists(Path.Combine(pathToFolderWithApp, localPathToMainExe)))
            {
                Console.WriteLine("Main executable does not exist in app directory.");
                return;
            }

            List<string> filesToPack = GetFilesRecursively(pathToFolderWithApp);
            for (int i = 0; i < filesToPack.Count; i++)
                filesToPack[i] = filesToPack[i].Replace(pathToFolderWithApp, string.Empty).TrimStart('\\');
        }

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
