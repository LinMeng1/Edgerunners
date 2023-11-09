using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NightCity.Core.Models.Standard;
using NightCity.Core.Services;
using NightCity.Daemon.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
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
        static void Main(string[] args)
        {
            httpService = new HttpService();
            sftpService = new SftpService("10.114.113.101", 2022, "NightCity", "nightcity");

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
            if (!runFlag)
            {
                await LaunchAsync();
            }
            ProcessTimer.Start();
        }

        /// <summary>
        /// 检查并更新NightCity后启动NightCity
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private static async Task LaunchAsync()
        {
            string installPath = string.Empty;
            try
            {
                List<LocalInstallInformation> infoList = InstalledPrograms.GetInstalledPrograms();
                foreach (var info in infoList)
                {
                    if (info.DisplayName == "NightCity")
                        installPath = Path.GetDirectoryName(info.DisplayIcon);
                }
            }
            catch { }
            if (installPath == string.Empty)
            {
                installPath = Path.GetDirectoryName(Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory));
                ControllersResult result = JsonConvert.DeserializeObject<ControllersResult>(await httpService.Post($"https://10.114.113.101/api/application/max-tac/publish/GetLatestRelease", new { Project = "NightCity" }));
                if (!result.Result)
                    throw new Exception(result.ErrorMessage);
                PublishInformation publish = JsonConvert.DeserializeObject<PublishInformation>(result.Content.ToString());
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
            Process.Start(Path.Combine(installPath, "NightCity.exe"));
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
