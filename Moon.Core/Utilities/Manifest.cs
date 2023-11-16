using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Renci.SshNet;
using System.Security.Cryptography;

namespace Moon.Core.Utilities
{
    public class Manifest
    {
        #region GetManifest
        private static string host = "10.114.113.101";
        private static int port = 2022;
        private static string username = "NightCity";
        private static string password = "nightcity";

        public static string GetManifest(string source, SftpClient? sftp = null)
        {
            JObject result = new();
            try
            {
                if (sftp == null)
                {
                    sftp = new SftpClient(host, port, username, password);
                    sftp.Connect();
                }
                using (sftp)
                {
                    var files = sftp.ListDirectory(source);
                    foreach (var file in files)
                    {
                        if (file.IsDirectory && file.GroupCanWrite)
                            result.Add(new JProperty(file.Name, GetManifest(file.FullName)));
                        else if (!file.IsDirectory)
                        {
                            Stream fs = sftp.OpenRead(file.FullName);
                            byte[] hash = new MD5CryptoServiceProvider().ComputeHash(fs);
                            fs.Close();
                            string hashStr = BitConverter.ToString(hash).Replace("-", string.Empty);
                            result.Add(new JProperty(file.Name, hashStr));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return JsonConvert.SerializeObject(result);
        }
        #endregion
    }
}
