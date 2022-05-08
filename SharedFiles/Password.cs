using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacker
{
    static class Password
    {
        internal static byte[] GetPasswordHash (string password) => Obfuscate(GetStringSha256Hash(password));

        internal static string GetPasswordHashString (string password) => BitConverter.ToString(GetPasswordHash(password)).Replace("-", "");
        internal static string GetPasswordHashString (byte[] passwordHash) => passwordHash == null ? string.Empty : BitConverter.ToString(passwordHash).Replace("-", "");

        internal static bool ComparePassword (string password, byte[] hash) => GetPasswordHash(password).SequenceEqual(hash);

        private static byte[] Obfuscate (byte[] data, byte xorConstant = 0x69)
        {
            for (int i = 0; i < data.Length; i++)
                data[i] = (byte)(data[i] ^ xorConstant);

            return data;
        }

        private static byte[] GetStringSha256Hash (string text)
        {
            if (string.IsNullOrEmpty(text))
                return new byte[] { };

            using (var sha = new System.Security.Cryptography.SHA256Managed())
            {
                byte[] textData = Encoding.UTF8.GetBytes(text);
                byte[] hash = sha.ComputeHash(textData);

                return hash;
            }
        }
    }
}
