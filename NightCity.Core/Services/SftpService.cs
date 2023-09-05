using Renci.SshNet;
using System;
using System.IO;
using System.Xml.Linq;

namespace NightCity.Core.Services
{
    public class SftpService
    {
        private string host;
        private int port;
        private string username;
        private string password;
        public SftpService(string host, int port, string username, string password)
        {
            this.host = host;
            this.port = port;
            this.username = username;
            this.password = password;
        }
        public void PullFiles(string source, string dest)
        {
            try
            {
                using (var sftp = new SftpClient(host, port, username, password))
                {
                    sftp.Connect();
                    var files = sftp.ListDirectory(source);
                    foreach (var file in files)
                    {
                        string name = file.Name;
                        if (!file.Name.StartsWith("."))
                        {
                            if (!Directory.Exists(dest))
                                Directory.CreateDirectory(dest);
                            using (Stream stream = File.Create($"{dest}/{name}"))
                            {
                                sftp.DownloadFile($"{source}/{name}", stream);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Global.Log($"[SftpService]:[PullFiles] exception:{e.Message}", true);
            }
        }
        public void PullFile(string source, string dest)
        {
            try
            {
                using (var sftp = new SftpClient(host, port, username, password))
                {
                    sftp.Connect();
                    using (Stream stream = File.Create(dest))
                    {
                        sftp.DownloadFile(source, stream);
                    }
                }
            }
            catch (Exception e)
            {
                Global.Log($"[SftpService]:[PullFile] exception:{e.Message}", true);
            }
        }
    }
}
