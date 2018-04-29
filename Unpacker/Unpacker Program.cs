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
        private static string tempDir = null;

        static void Main(string[] args)
        {

#if DEBUG
            Console.WriteLine("Running in Debug");
            Console.WriteLine("Attach to process now and press Enter...");
            Console.ReadLine();
            Console.WriteLine("Resuming");
#endif

            // Subscribe for exit event to delete tempDir
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(Unpacker_ProcessExit);

            // Create temp directory to store unpacked files
            while (tempDir == null || Directory.Exists(tempDir))
                tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDir);

            // Open self exe file as byte stream and unpack data that is appended to the end of exe
            string selfExePath = System.Reflection.Assembly.GetEntryAssembly().Location;
            string pathToMainExe = null;
            using (var me = new BinaryReader(File.OpenRead(selfExePath)))
            {
                // Find the end of the actual .exe
                byte[] pattern = Encoding.UTF8.GetBytes("<SerGreen>");
                long pos = FindPatternPosition(me.BaseStream, pattern);
                me.BaseStream.Seek(pos + pattern.Length, SeekOrigin.Begin);
                
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
            if (tempDir != null && Directory.Exists(tempDir))
                Directory.Delete(tempDir, true);
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
    }
}
