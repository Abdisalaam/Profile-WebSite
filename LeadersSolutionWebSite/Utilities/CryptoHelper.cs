using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace LeadersSolutionWebSite.Utilities
{
    public static class CryptoHelper
    {
        // Original string values (kept for human readability / rotation)
        private static readonly string Key = "A7dP9sQ4vT2kL8mN3rX6bC1fH5jW0zY";
        private static readonly string IV = "Q9wL3tB7rF2kM6pN";

        // Derive a 32-byte key from Key (SHA256) and a 16-byte IV (first 16 bytes of SHA256(IV))
        private static byte[] DeriveKeyBytes()
        {
            using (var sha = SHA256.Create())
            {
                return sha.ComputeHash(Encoding.UTF8.GetBytes(Key));
            }
        }

        private static byte[] DeriveIVBytes()
        {
            using (var sha = SHA256.Create())
            {
                var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(IV));
                var iv = new byte[16];
                Array.Copy(hash, iv, 16);
                return iv;
            }
        }

        // Diagnostic helper: returns runtime provider info and legal key sizes
        public static string GetAesProviderInfo()
        {
            using (Aes aes = Aes.Create())
            {
                var sb = new StringBuilder();
                sb.AppendLine(aes.GetType().FullName);
                foreach (var ks in aes.LegalKeySizes)
                {
                    sb.AppendFormat("LegalKeySizes: Min={0}, Max={1}, Skip={2}\n", ks.MinSize, ks.MaxSize, ks.SkipSize);
                }
                return sb.ToString();
            }
        }

        public static string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                return plainText;

            var keyBytes = DeriveKeyBytes();
            var ivBytes = DeriveIVBytes();

            using (Aes aes = Aes.Create())
            {
                aes.Key = keyBytes;
                aes.IV = ivBytes;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using (var ms = new MemoryStream())
                using (var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                using (var sw = new StreamWriter(cs, Encoding.UTF8))
                {
                    sw.Write(plainText);
                    sw.Close();
                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }

        public static string Decrypt(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText))
                return cipherText;

            var keyBytes = DeriveKeyBytes();
            var ivBytes = DeriveIVBytes();

            using (Aes aes = Aes.Create())
            {
                aes.Key = keyBytes;
                aes.IV = ivBytes;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using (var ms = new MemoryStream(Convert.FromBase64String(cipherText)))
                using (var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read))
                using (var sr = new StreamReader(cs, Encoding.UTF8))
                {
                    return sr.ReadToEnd();
                }
            }
        }

        public static string Hash(string input)
        {
            using (var sha = SHA256.Create())
            {
                var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
                return Convert.ToBase64String(bytes);
            }
        }

    }
}