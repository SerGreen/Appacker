using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unpacker
{
    class Program
    {
        private const int WAIT_FOR_FILE_ACCESS_TIMEOUT = 5000; // ms
        private static string tempDir = null;
        private static string repackerTempDir = null;
        private static bool isSelfRepackable = false;
        private static string pathToMainExe = null;

        static void Main(string[] args)
        {
#if DEBUG
            Console.WriteLine("Unpacker is running in Debug");
            Console.WriteLine("Attach to process now and press Enter...");
            Console.ReadLine();
            Console.WriteLine("Resuming");
#endif

            // After self-repacking, packer.exe can't delete itself from temp dir, so it calls repacked app to kill it
            // Command is "unpacker.exe -killme <path to packer.exe>"
            // The whole folder where packer.exe is gets deleted (cuz there should also be temp unpacker.exe from repacking process)
            if(args.Length > 0 && 
                (args[0] == "-killme" || args[0] == "killme"))
            {
                if(args.Length > 1)
                {
                    if(WaitForFileAccess(args[1], WAIT_FOR_FILE_ACCESS_TIMEOUT) == true)
                    {
                        Directory.Delete(Path.GetDirectoryName(args[1]), true);
                    }
                }

                // Just close the app now
                return;
            }

            // Subscribe for exit event to delete tempDir
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(Unpacker_ProcessExit);

            // Create temp directory to store unpacked files
            while (tempDir == null || Directory.Exists(tempDir))
                tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDir);

            // Open self exe file as byte stream and unpack data that is appended to the end of exe
            string selfExePath = System.Reflection.Assembly.GetEntryAssembly().Location;
            using (var me = new BinaryReader(File.OpenRead(selfExePath)))
            {
                // Find the end of the actual unpacker.exe
                byte[] pattern = Encoding.UTF8.GetBytes("<SerGreen>");
                long pos = FindPatternPosition(me.BaseStream, pattern);
                me.BaseStream.Seek(pos + pattern.Length, SeekOrigin.Begin);

                isSelfRepackable = me.ReadBoolean();
                if(isSelfRepackable)
                {
                    // Create temp directory to store packer.exe and unpacker.exe for repacking process
                    while (repackerTempDir == null || Directory.Exists(repackerTempDir))
                        repackerTempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                    Directory.CreateDirectory(repackerTempDir);

                    // Return to the beginning of the file and save the unpacker.exe part
                    me.BaseStream.Seek(0, SeekOrigin.Begin);
                    File.WriteAllBytes(Path.Combine(repackerTempDir, "unpacker.exe"), me.ReadBytes((int)pos));
                    // Skip <SerGreen> keyword and 1 byte of isSelfRepackable flag
                    me.ReadBytes(pattern.Length + 1);
                    // Save the packer.exe
                    int packerDataLength = me.ReadInt32();
                    File.WriteAllBytes(Path.Combine(repackerTempDir, "packer.exe"), me.ReadBytes(packerDataLength));
                }

                pathToMainExe = me.ReadString();

                // Extract files until the end of stream is reached
                while(me.BaseStream.Position < me.BaseStream.Length)
                {
                    string path = me.ReadString();
                    int length = me.ReadInt32();
                    byte[] data = me.ReadBytes(length);

                    string dir = Path.GetDirectoryName(path);
                    if (!string.IsNullOrWhiteSpace(dir) && !Directory.Exists(Path.Combine(tempDir, dir)))
                        Directory.CreateDirectory(Path.Combine(tempDir, dir));

                    File.WriteAllBytes(Path.Combine(tempDir, path), data);
                }
            }

            // Launch unpacked app
            Process proc = Process.Start(Path.Combine(tempDir, pathToMainExe));

            // Wait until app exits to delete its files from temp directory
            proc.WaitForExit();
        }

        // Exit event. Delete tempDir recursively
        private static void Unpacker_ProcessExit(object sender, EventArgs e)
        {
            // If it's a self-repackable app, then launch packer.exe from temp directory
            if (isSelfRepackable)
            {
                // Launch packer.exe with arguments:
                // 1. Path to unpacker.exe (inside repackTempDir)
                // 2. Path where to save packed app (path of current exe)
                // 3. Relative path to main executable inside app directory
                // 4. Path to app directory (tempDir)
                // 5. True flag = this app is self-repackable
                // 6. -repack flag = this is the repacking process, which will result in deletion of unpacked app folder after repacking
                ProcessStartInfo repackProcInfo = new ProcessStartInfo(Path.Combine(repackerTempDir, "packer.exe"));
                repackProcInfo.Arguments = $@"""{Path.Combine(repackerTempDir, "unpacker.exe")}"" ""{System.Reflection.Assembly.GetEntryAssembly().Location}"" ""{pathToMainExe}"" ""{tempDir}"" True -repack";
#if (!DEBUG)
                repackProcInfo.CreateNoWindow = true;
                repackProcInfo.WindowStyle = ProcessWindowStyle.Hidden;
#endif
                Process.Start(repackProcInfo);
            }
            // If it's not a repackable app, then delete temp files immediately
            else
            {
                if (tempDir != null && Directory.Exists(tempDir))
                    Directory.Delete(tempDir, true);
            }
        }


#region Find pattern position in stream methods
        // Solution from here https://stackoverflow.com/questions/1471975
        /// <summary>
        /// Search for first occurance of byte-sequence pattern in stream
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
