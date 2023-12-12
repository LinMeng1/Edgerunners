using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NightCity.Core.Models.Standard;
using NightCity.Core.Services;
using NightCity.Core.Utilities;
using NightCity.Daemon.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Net.Security;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace NightCity.Daemon
{
    internal static class Program
    {
        //Http服务
        private static HttpService httpService;
        //Sftp服务
        private static SftpService sftpService;
        private static Mutex _mutex = null;
        private static System.Timers.Timer ProcessTimer;
        private static DateTime lastVersionCheck;
        static void Main(string[] args)
        {
            ServicePointManager.ServerCertificateValidationCallback = delegate (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
            {
                return true;
            };

            httpService = new HttpService();
            sftpService = new SftpService("10.114.113.101", 2022, Encryption.DecryptString("tonxrWIi+Dq/73qTwIQEKQ=="), Encryption.DecryptString("fxUxc7Rrk3op/6F1bBdmLw=="));

            #region 防止复数实例
            const string appName = "NightCity Daemon";
            _mutex = new Mutex(true, appName, out bool createdNew);
            if (!createdNew)
                return;
            #endregion

            #region 进程守护
            ProcessTimer = new System.Timers.Timer(100)
            {
                Enabled = true
            };
            ProcessTimer.Elapsed += ProcessMethod;
            ProcessTimer.Start();
            #endregion

            new ManualResetEvent(false).WaitOne();

        }
        private static async void ProcessMethod(object sender, ElapsedEventArgs e)
        {
            try
            {
                if (ProcessTimer.Interval != 3000)
                    ProcessTimer.Interval = 3000;
                ProcessTimer.Stop();
                bool runFlag = false;
                foreach (Process p in Process.GetProcesses())
                {
                    if (p.ProcessName == "NightCity")
                    {
                        runFlag = true;
                    }
                }
                if (!runFlag || (DateTime.Now - lastVersionCheck).TotalHours > 6)
                {
                    await LaunchAsync();
                }
                ProcessTimer.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// 检查并更新NightCity后启动NightCity
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private static async Task LaunchAsync()
        {
            PublishInformation publish = null;
            string installPath = string.Empty;
            try
            {
                try
                {
                    ControllersResult result = JsonConvert.DeserializeObject<ControllersResult>(await httpService.Post($"https://10.114.113.101/api/application/max-tac/publish/GetLatestRelease", new { Project = "NightCity" }));
                    if (!result.Result)
                        throw new Exception(result.ErrorMessage);
                    publish = JsonConvert.DeserializeObject<PublishInformation>(result.Content.ToString());
                }
                catch { }
                try
                {
                    List<LocalInstallInformation> infoList = InstalledPrograms.GetInstalledPrograms();
                    foreach (var info in infoList)
                    {
                        if (info.DisplayName == "NightCity")
                        {
                            if (publish == null || info.DisplayVersion == publish.Version)
                                installPath = Path.GetDirectoryName(info.DisplayIcon);
                            else
                            {
                                foreach (Process p in Process.GetProcesses())
                                {
                                    if (p.ProcessName.CompareTo(info.DisplayName) == 0)
                                    {
                                        p.Kill();
                                        break;
                                    }
                                }
                                Thread.Sleep(2000);
                                InstalledPrograms.RemoveUninstallInRegistry(info);
                                Directory.Delete(Path.GetDirectoryName(info.DisplayIcon), true);
                            }
                            break;
                        }
                    }
                }
                catch { }
                try
                {
                    if (installPath == string.Empty)
                    {
                        installPath = Path.GetDirectoryName(Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory));
                        installPath = Path.Combine(installPath, "NightCity");
                        await Task.Run(() =>
                        {
                            LocalInstallInformation information = new LocalInstallInformation()
                            {
                                DisplayName = "NightCity"
                            };
                            KillProduct(information);
                            Directory.CreateDirectory(installPath);
                            JToken mainfest = JToken.Parse(publish.Manifest);
                            sftpService.SyncFiles(mainfest, publish.ReleaseAddress, string.Empty, installPath);
                            information.DisplayVersion = publish.Version;
                            information.Publisher = "LinMeng";
                            information.DisplayIcon = Path.Combine(installPath, $"{information.DisplayName}.exe");
                            information.UninstallString = Path.Combine(Path.Combine(Path.Combine(Path.GetDirectoryName(installPath), "NightCity Launcher"), "NightCity Launcher.exe"));
                            InstalledPrograms.CreateUninstallInRegistry(information);
                        });
                    }
                }
                catch { }
                Process.Start(Path.Combine(installPath, "NightCity.exe"));
                lastVersionCheck = DateTime.Now;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        private async static void KillProduct(LocalInstallInformation information)
        {
            try
            {
                try
                {
                    if (information.DisplayName == "NightCity")
                    {
                        NamedPipeClientStream pipeClient = new NamedPipeClientStream("localhost", "NightCityPipe", PipeDirection.InOut, PipeOptions.Asynchronous, TokenImpersonationLevel.None);
                        pipeClient.Connect(1000);
                        StreamWriter sw = new StreamWriter(pipeClient)
                        {
                            AutoFlush = true
                        };
                        sw.WriteLine("NightCity Hide");
                        pipeClient.Dispose();
                        await Task.Delay(1000);
                    }
                }
                catch { }
                foreach (Process p in Process.GetProcesses())
                {
                    if (p.ProcessName.CompareTo(information.DisplayName) == 0)
                    {
                        p.Kill();
                        break;
                    }
                }
            }
            catch { }
        }
    }
}
