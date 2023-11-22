using System.Security.Cryptography;
using System.Text;

namespace Moon.Core.Utilities
{
    public static class Encryption
    {
        private static readonly byte[] Key = Encoding.UTF8.GetBytes("LetYouDown123456"); // 设置加密密钥，可以根据需要修改
        private static readonly byte[] IV = Encoding.UTF8.GetBytes("ThisFffire123456"); // 设置加密向量，可以根据需要修改
        public static string DecryptString(string encryptedStr)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = Key;
                aes.IV = IV;

                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                byte[] encryptedBytes = Convert.FromBase64String(encryptedStr);
                byte[] decryptedBytes = new byte[encryptedBytes.Length];

                using (MemoryStream memoryStream = new MemoryStream(encryptedBytes))
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        cryptoStream.Read(decryptedBytes, 0, decryptedBytes.Length);
                    }
                }

                return Encoding.UTF8.GetString(decryptedBytes).Replace("\0", ""); ;
            }
        }
    }
}
