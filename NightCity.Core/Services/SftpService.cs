using Newtonsoft.Json.Linq;
using Renci.SshNet;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
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
        public void PullFiles(string source, string dest, SftpClient sftp = null)
        {
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
                            PullFiles(file.FullName, $@"{dest}\{file.Name}", sftp);
                        else
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
        public void SyncFiles(JToken manifest, string publishDirectory, string relativeDirectory, string distDirectory)
        {
            foreach (JProperty file in manifest.Cast<JProperty>())
            {
                JToken _file = manifest[file.Name];
                if (_file.Type == JTokenType.Object)
                {
                    SyncFiles(_file, publishDirectory, $"/{file.Name}", distDirectory);
                }
                else if (_file.Type == JTokenType.String)
                {
                    if (File.Exists($"{distDirectory}{relativeDirectory}/{file.Name}"))
                    {
                        FileStream fsLocal = new FileStream($"{distDirectory}{relativeDirectory}/{file.Name}", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                        byte[] hashLocal = new MD5CryptoServiceProvider().ComputeHash(fsLocal);
                        fsLocal.Close();
                        string hashLocalStr = BitConverter.ToString(hashLocal).Replace("-", string.Empty);
                        string hashSourceStr = _file.ToString();
                        if (hashLocalStr != hashSourceStr)
                        {
                            PullFile($"{publishDirectory}{relativeDirectory}/{file.Name}", $"{distDirectory}{relativeDirectory}/{file.Name}");
                        }
                    }
                    else
                    {
                        Directory.CreateDirectory($"{distDirectory}{relativeDirectory}");
                        PullFile($"{publishDirectory}{relativeDirectory}/{file.Name}", $"{distDirectory}{relativeDirectory}/{file.Name}");
                    }
                }
            }
        }
    }
}
