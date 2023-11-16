using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using Newtonsoft.Json;
using NightCity.Core;
using NightCity.Core.Events;
using NightCity.Core.Models;
using NightCity.Core.Models.Standard;
using NightCity.Core.Services;
using NightCity.Core.Services.Prism;
using OnCall.Models;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace OnCall.ViewModels
{
    public class MainViewModel : BindableBase, IDisposable
    {
        //事件聚合器
        private IEventAggregator eventAggregator;
        //属性服务
        private IPropertyService propertyService;
        //Http服务
        private HttpService httpService;
        //监听token列表
        List<SubscriptionToken> eventTokens = new List<SubscriptionToken>();
        public MainViewModel(IEventAggregator eventAggregator, IPropertyService propertyService)
        {
            //依赖注入及初始化
            this.eventAggregator = eventAggregator;
            this.propertyService = propertyService;
            httpService = new HttpService();
            //获取Token   
            httpService.AddToken(propertyService.GetProperty("TestEngAuthorizationInfo")?.ToString());
            //监听事件 权限信息变更
            eventTokens.Add(eventAggregator.GetEvent<AuthorizationInfoChangedEvent>().Subscribe((authorizationInfo) =>
            {
                object token = propertyService.GetProperty(authorizationInfo.Item2);
                httpService.AddToken(token?.ToString());
            }, ThreadOption.UIThread, true));
            //监听事件 链接命令发布
            eventTokens.Add(eventAggregator.GetEvent<BannerMessageLinkingEvent>().Subscribe(async message =>
            {
                string patternSync = "module on-call sync issue";
                if (message == patternSync)
                {
                    View = "Workshop";
                    eventAggregator.GetEvent<TemplateShowingEvent>().Publish("OnCall");
                    await SyncOpenReportsAsync();
                    return;
                }
                string patternResponse = @"module on-call response issue ([a-z0-9-]*)";
                string patternSolve = @"module on-call solve issue ([a-z0-9-]*)";
                Regex regexResponse = new Regex(patternResponse, RegexOptions.IgnoreCase);
                Regex regexSolve = new Regex(patternSolve, RegexOptions.IgnoreCase);
                Match mResponse = regexResponse.Match(message);
                Match mSolve = regexSolve.Match(message);
                if (mResponse.Success)
                    await ResponseReportAsync(mResponse.Groups[1].Value, true);
                else if (mSolve.Success)
                {
                    View = "Workshop";
                    SolvedReportId = mSolve.Groups[1].Value;
                    eventAggregator.GetEvent<TemplateShowingEvent>().Publish("OnCall");
                    TrySolveReport();
                }
            }, ThreadOption.UIThread, true));
            SyncOpenReports();
            GetAllReports();

            #region P1
            SeriesCollection1 = new SeriesCollection
            {
                new StackedAreaSeries
                {
                    Title = "Africa",
                    Values = new ChartValues<DateTimePoint>
                    {
                        new DateTimePoint(new DateTime(1950, 1, 1), .228),
                        new DateTimePoint(new DateTime(1960, 1, 1), .285),
                        new DateTimePoint(new DateTime(1970, 1, 1), .366),
                        new DateTimePoint(new DateTime(1980, 1, 1), .478),
                        new DateTimePoint(new DateTime(1990, 1, 1), .629),
                        new DateTimePoint(new DateTime(2000, 1, 1), .808),
                        new DateTimePoint(new DateTime(2010, 1, 1), 1.031),
                        new DateTimePoint(new DateTime(2013, 1, 1), 1.110)
                    },
                    LineSmoothness = 0
                },
                new StackedAreaSeries
                {
                    Title = "N & S America",
                    Values = new ChartValues<DateTimePoint>
                    {
                        new DateTimePoint(new DateTime(1950, 1, 1), .339),
                        new DateTimePoint(new DateTime(1960, 1, 1), .424),
                        new DateTimePoint(new DateTime(1970, 1, 1), .519),
                        new DateTimePoint(new DateTime(1980, 1, 1), .618),
                        new DateTimePoint(new DateTime(1990, 1, 1), .727),
                        new DateTimePoint(new DateTime(2000, 1, 1), .841),
                        new DateTimePoint(new DateTime(2010, 1, 1), .942),
                        new DateTimePoint(new DateTime(2013, 1, 1), .972)
                    },
                    LineSmoothness = 0
                },
                new StackedAreaSeries
                {
                    Title = "Asia",
                    Values = new ChartValues<DateTimePoint>
                    {
                        new DateTimePoint(new DateTime(1950, 1, 1), 1.395),
                        new DateTimePoint(new DateTime(1960, 1, 1), 1.694),
                        new DateTimePoint(new DateTime(1970, 1, 1), 2.128),
                        new DateTimePoint(new DateTime(1980, 1, 1), 2.634),
                        new DateTimePoint(new DateTime(1990, 1, 1), 3.213),
                        new DateTimePoint(new DateTime(2000, 1, 1), 3.717),
                        new DateTimePoint(new DateTime(2010, 1, 1), 4.165),
                        new DateTimePoint(new DateTime(2013, 1, 1), 4.298)
                    },
                    LineSmoothness = 0
                },
                new StackedAreaSeries
                {
                    Title = "Europe",
                    Values = new ChartValues<DateTimePoint>
                    {
                        new DateTimePoint(new DateTime(1950, 1, 1), .549),
                        new DateTimePoint(new DateTime(1960, 1, 1), .605),
                        new DateTimePoint(new DateTime(1970, 1, 1), .657),
                        new DateTimePoint(new DateTime(1980, 1, 1), .694),
                        new DateTimePoint(new DateTime(1990, 1, 1), .723),
                        new DateTimePoint(new DateTime(2000, 1, 1), .729),
                        new DateTimePoint(new DateTime(2010, 1, 1), .740),
                        new DateTimePoint(new DateTime(2013, 1, 1), .742)
                    },
                    LineSmoothness = 0
                }
            };
            XFormatter1 = val => new DateTime((long)val).ToString("yyyy");
            YFormatter1 = val => val.ToString("N") + " M";
            #endregion

            #region P2
            SeriesCollection2 = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "2015",
                    Values = new ChartValues<double> { 10, 50, 39, 50 }
                }
            };

            //adding series will update and animate the chart automatically
            SeriesCollection2.Add(new ColumnSeries
            {
                Title = "2016",
                Values = new ChartValues<double> { 11, 56, 42 }
            });

            //also adding values updates and animates the chart automatically
            SeriesCollection2[1].Values.Add(48d);

            Labels2 = new[] { "Maria", "Susan", "Charles", "Frida" };
            Formatter2 = value => value.ToString("N");
            #endregion

            #region P3
            SeriesCollection3 = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Series 1",
                    Values = new ChartValues<double> { 4, 6, 5, 2 ,4 }
                },
                new LineSeries
                {
                    Title = "Series 2",
                    Values = new ChartValues<double> { 6, 7, 3, 4 ,6 },
                    PointGeometry = null
                },
                new LineSeries
                {
                    Title = "Series 3",
                    Values = new ChartValues<double> { 4,2,7,2,7 },
                    PointGeometry = DefaultGeometries.Square,
                    PointGeometrySize = 15
                }
            };

            Labels3 = new[] { "Jan", "Feb", "Mar", "Apr", "May" };
            YFormatter3 = value => value.ToString("C");
            #endregion

            #region P4
            PointLabel4 = chartPoint => string.Format("{0} ({1:P})", chartPoint.Y, chartPoint.Participation);
            #endregion

            #region P5
            SeriesCollection5 = new SeriesCollection
            {
                new OhlcSeries()
                {
                    Values = new ChartValues<OhlcPoint>
                    {
                        new OhlcPoint(32, 35, 30, 32),
                        new OhlcPoint(33, 38, 31, 37),
                        new OhlcPoint(35, 42, 30, 40),
                        new OhlcPoint(37, 40, 35, 38),
                        new OhlcPoint(35, 38, 32, 33)
                    }
                },
                new LineSeries
                {
                    Values = new ChartValues<double> {30, 32, 35, 30, 28},
                    Fill = Brushes.Transparent
                }
            };

            Labels5 = new[]
            {
                DateTime.Now.ToString("dd MMM"),
                DateTime.Now.AddDays(1).ToString("dd MMM"),
                DateTime.Now.AddDays(2).ToString("dd MMM"),
                DateTime.Now.AddDays(3).ToString("dd MMM"),
                DateTime.Now.AddDays(4).ToString("dd MMM"),
            };
            #endregion

        }

        /// <summary>
        /// 同步进行中的异常报告
        /// </summary>
        /// <returns></returns>
        private async Task SyncOpenReportsAsync()
        {
            try
            {
                MessageHost.Show();
                MessageHost.DialogCategory = "Syncing";
                await Task.Delay(MessageHost.InternalDelay);
                object mainboard = propertyService.GetProperty("Mainboard") ?? throw new Exception("Mainboard is null");
                ControllersResult result = JsonConvert.DeserializeObject<ControllersResult>(await httpService.Post("https://10.114.113.101/api/application/night-city/modules/on-call/GetOpenReports", new { Mainboard = mainboard.ToString() }));
                if (!result.Result)
                    throw new Exception(result.ErrorMessage);
                GetOpenReports reports = JsonConvert.DeserializeObject<GetOpenReports>(result.Content.ToString());
                LocalOpenReportList = reports.LocalRepairs;
                ClusterOpenReportList = reports.ClusterRepairs;
                MessageHost.Hide();
            }
            catch (Exception e)
            {
                Global.Log($"[OnCall]:[MainViewModel]:[SyncOpenReportsAsync]:exception:{e.Message}", true);
                MessageHost.DialogMessage = e.Message;
                MessageHost.DialogCategory = "Message";
            }
        }

        /// <summary>
        /// 查询所有异常报告
        /// </summary>
        /// <returns></returns>
        private async Task GetAllReportsAsync()
        {
            try
            {
                MessageHost.Show();
                MessageHost.DialogCategory = "Syncing";
                await Task.Delay(MessageHost.InternalDelay);
                ControllersResult result = JsonConvert.DeserializeObject<ControllersResult>(await httpService.Get("https://10.114.113.101/api/application/night-city/modules/on-call/GetAllReports"));
                if (!result.Result)
                    throw new Exception(result.ErrorMessage);
                List<GetAllReports_C1> reports = JsonConvert.DeserializeObject<List<GetAllReports_C1>>(result.Content.ToString());
                foreach (var report in reports)
                {
                    switch (report.State)
                    {
                        case "triggered":
                            report.DisplayName = report.InitialOwnerName;
                            report.TimeCost = $">{Math.Round((DateTime.Now - report.TriggerTime).TotalHours),1}H";
                            break;
                        case "responsed":
                            report.DisplayName = report.ResponserName;
                            report.TimeCost = $">{Math.Round((DateTime.Now - report.TriggerTime).TotalHours, 1)}H";
                            break;
                        case "solved":
                            report.DisplayName = report.SolverName;
                            report.DisplayDescription = report.FailureCategory;
                            if (report.SolveTime != null)
                                report.TimeCost = $"{Math.Round(((TimeSpan)(report.SolveTime - report.TriggerTime)).TotalHours, 1)}H";
                            break;
                        case "aborted":
                            report.DisplayName = report.SolverName;
                            report.DisplayDescription = report.Solution;
                            if (report.SolveTime != null)
                                report.TimeCost = $"{Math.Round(((TimeSpan)(report.SolveTime - report.TriggerTime)).TotalHours, 1)}H";
                            break;
                        default:
                            break;
                    }
                }
                AllReportList = reports;
                MessageHost.Hide();
            }
            catch (Exception e)
            {
                Global.Log($"[OnCall]:[MainViewModel]:[SyncHistoricalReportsAsync]:exception:{e.Message}", true);
                MessageHost.DialogMessage = e.Message;
                MessageHost.DialogCategory = "Message";
            }
        }

        /// <summary>
        /// 异常接单
        /// </summary>
        /// <param name="reportId"></param>
        /// <returns></returns>
        private async Task ResponseReportAsync(string reportId, bool messagebox = false)
        {
            try
            {
                View = "Workshop";
                MessageHost.Show();
                MessageHost.DialogCategory = "Syncing";
                await Task.Delay(MessageHost.InternalDelay);
                ControllersResult result = JsonConvert.DeserializeObject<ControllersResult>(await httpService.Post("https://10.114.113.101/api/application/night-city/modules/on-call/HandleReport", new { ReportId = reportId, }));
                if (!result.Result)
                    throw new Exception(result.ErrorMessage);
                List<string> jurisdictionalClusterOwner = JsonConvert.DeserializeObject<List<string>>(result.Content.ToString());
                eventAggregator.GetEvent<MqttMessageSendingEvent>().Publish(new Tuple<List<string>, MqttMessage>(jurisdictionalClusterOwner, new MqttMessage()
                {
                    IsMastermind = true,
                    Content = "system sync banner messages"
                }));
                await SyncOpenReportsAsync();
                await GetAllReportsAsync();
                MessageHost.Hide();
            }
            catch (Exception e)
            {
                Global.Log($"[OnCall]:[MainViewModel]:[CheckIssueStateAsync] exception:{e.Message}", true);
                if (messagebox)
                {
                    eventAggregator.GetEvent<ErrorMessageShowingEvent>().Publish(new Tuple<string, string>(e.Message, "OnCall"));
                    MessageHost.Hide();
                }
                else
                {
                    MessageHost.DialogMessage = e.Message;
                    MessageHost.DialogCategory = "Message";
                }
            }
        }

        /// <summary>
        /// 异常解决
        /// </summary>
        /// <param name="reportId"></param>
        /// <param name="product"></param>
        /// <param name="process"></param>
        /// <param name="failureCategory"></param>
        /// <param name="failureReason"></param>
        /// <param name="solution"></param>
        /// <returns></returns>
        private async Task SolveReportAsync(string reportId, string product, string process, string failureCategory, string failureReason, string solution)
        {
            try
            {
                MessageHost.Show();
                MessageHost.DialogCategory = "Syncing";
                await Task.Delay(MessageHost.InternalDelay);
                if (product == null || product == string.Empty)
                    throw new Exception("Product is empty");
                if (process == null || process == string.Empty)
                    throw new Exception("Process is empty");
                if (failureCategory == null || failureCategory == string.Empty)
                    throw new Exception("FailureCategory is empty");
                List<object> attachements = new List<object>();
                foreach (var attachment in Attachments)
                {
                    attachements.Add(new
                    {
                        attachment.Name,
                        attachment.Extension,
                        attachment.Base64Str
                    });
                }
                ControllersResult result = JsonConvert.DeserializeObject<ControllersResult>(await httpService.Post("https://10.114.113.101/api/application/night-city/modules/on-call/HandleReport", new { ReportId = reportId, Product = product, Process = process, FailureCategory = failureCategory, FailureReason = failureReason, Solution = solution, Attachments = attachements }));
                if (!result.Result)
                    throw new Exception(result.ErrorMessage);
                List<string> jurisdictionalClusters = JsonConvert.DeserializeObject<List<string>>(result.Content.ToString());
                eventAggregator.GetEvent<MqttMessageSendingEvent>().Publish(new Tuple<List<string>, MqttMessage>(jurisdictionalClusters, new MqttMessage()
                {
                    IsMastermind = true,
                    Content = "system sync banner messages"
                }));
                await SyncOpenReportsAsync();
                await GetAllReportsAsync();
                MessageHost.Hide();
            }
            catch (Exception e)
            {
                Global.Log($"[OnCall]:[MainViewModel]:[SolveReportAsync]:exception:{e.Message}", true);
                MessageHost.DialogMessage = e.Message;
                MessageHost.DialogCategory = "Message";
            }
        }

        /// <summary>
        /// 误报提交
        /// </summary>
        /// <param name="reportId"></param>
        /// <returns></returns>
        private async Task MisReportAsync(string reportId)
        {
            try
            {
                MessageHost.Show();
                MessageHost.DialogCategory = "Syncing";
                await Task.Delay(MessageHost.InternalDelay);
                ControllersResult result = JsonConvert.DeserializeObject<ControllersResult>(await httpService.Post("https://10.114.113.101/api/application/night-city/modules/on-call/HandleReport", new { ReportId = reportId, AbortReason = "MisReport" }));
                if (!result.Result)
                    throw new Exception(result.ErrorMessage);
                List<string> jurisdictionalClusters = JsonConvert.DeserializeObject<List<string>>(result.Content.ToString());
                eventAggregator.GetEvent<MqttMessageSendingEvent>().Publish(new Tuple<List<string>, MqttMessage>(jurisdictionalClusters, new MqttMessage()
                {
                    IsMastermind = true,
                    Content = "system sync banner messages"
                }));
                await SyncOpenReportsAsync();
                await GetAllReportsAsync();
                MessageHost.Hide();
            }
            catch (Exception e)
            {
                Global.Log($"[OnCall]:[MainViewModel]:[MisReportAsync]:exception:{e.Message}", true);
                MessageHost.DialogMessage = e.Message;
                MessageHost.DialogCategory = "Message";
            }
        }

        /// <summary>
        /// 异常报告导出至Excel
        /// </summary>
        /// <returns></returns>
        private async Task ExportReportsToExcelAsync(List<GetAllReports_C1> reports, string ExcelFilePath = null)
        {
            try
            {
                MessageHost.Show();
                MessageHost.DialogCategory = "Syncing";
                await Task.Delay(MessageHost.InternalDelay);
                await Task.Run(() =>
                {
                    Microsoft.Office.Interop.Excel.Application Excel = new Microsoft.Office.Interop.Excel.Application();
                    Excel.Workbooks.Add();
                    Microsoft.Office.Interop.Excel._Worksheet Worksheet = Excel.ActiveSheet;
                    var columns = new GetAllReports_C1().GetType().GetProperties();
                    var columnsCount = columns.Count();
                    object[] Header = new object[columnsCount];
                    for (int i = 0; i < columnsCount; i++)
                        Header[i] = columns[i].Name;
                    Microsoft.Office.Interop.Excel.Range HeaderRange = Worksheet.get_Range((Microsoft.Office.Interop.Excel.Range)Worksheet.Cells[1, 1], (Microsoft.Office.Interop.Excel.Range)(Worksheet.Cells[1, columnsCount]));
                    HeaderRange.Value = Header;
                    HeaderRange.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                    HeaderRange.Font.Bold = true;
                    int rowsCount = reports.Count;
                    object[,] Cells = new object[rowsCount, columnsCount];
                    for (int j = 0; j < rowsCount; j++)
                        for (int i = 0; i < columnsCount; i++)
                            Cells[j, i] = columns[i].GetValue(reports[j]);
                    Worksheet.get_Range((Microsoft.Office.Interop.Excel.Range)(Worksheet.Cells[2, 1]), (Microsoft.Office.Interop.Excel.Range)(Worksheet.Cells[rowsCount + 1, columnsCount])).Value = Cells;
                    Worksheet.Columns.AutoFit();
                    if (ExcelFilePath != null && ExcelFilePath != "")
                    {
                        try
                        {
                            Worksheet.SaveAs(ExcelFilePath);
                            Excel.Quit();
                            throw new Exception("Excel file saved!");
                        }
                        catch (Exception ex)
                        {
                            throw new Exception($"ExportToExcel: Excel file could not be saved! Check filepath: {ex.Message}");
                        }
                    }
                    else
                    {
                        Excel.Visible = true;
                    }
                });
                MessageHost.Hide();
            }
            catch (Exception e)
            {
                Global.Log($"[OnCall]:[MainViewModel]:[ExportReportsToExcelAsync]:exception:{e.Message}", true);
                MessageHost.DialogMessage = e.Message;
                MessageHost.DialogCategory = "Message";
            }
        }

        /// <summary>
        /// 查询产品列表
        /// </summary>
        /// <returns></returns>
        private async Task GetProductListAsyncBack()
        {
            try
            {
                await Task.Delay(MessageHost.InternalDelay);
                ControllersResult result = JsonConvert.DeserializeObject<ControllersResult>(await httpService.Get("https://10.114.113.101/api/application/night-city/modules/product/GetProductList"));
                if (!result.Result)
                    throw new Exception(result.ErrorMessage);
                List<string> list = JsonConvert.DeserializeObject<List<string>>(result.Content.ToString());
                ProductList = list;
            }
            catch (Exception e)
            {
                Global.Log($"[OnCall]:[MainViewModel]:[GetProductListAsyncBack]:exception:{e.Message}", true);
            }
        }

        /// <summary>
        /// 查询故障类型列表
        /// </summary>
        /// <returns></returns>
        private async Task GetFailureCategoryListAsyncBack()
        {
            try
            {
                await Task.Delay(MessageHost.InternalDelay);
                ControllersResult result = JsonConvert.DeserializeObject<ControllersResult>(await httpService.Get("https://10.114.113.101/api/application/night-city/modules/on-call/GetFailureCategoryList"));
                if (!result.Result)
                    throw new Exception(result.ErrorMessage);
                List<string> list = JsonConvert.DeserializeObject<List<string>>(result.Content.ToString());
                FailureCategoryList = list;
            }
            catch (Exception e)
            {
                Global.Log($"[OnCall]:[MainViewModel]:[GetFailureCategoryListAsyncBack]:exception:{e.Message}", true);
            }
        }

        /// <summary>
        /// 查询产品流程列表
        /// </summary>
        /// <returns></returns>
        private async Task GetProductProcessListAsyncBack()
        {
            try
            {
                await Task.Delay(MessageHost.InternalDelay);
                ControllersResult result = JsonConvert.DeserializeObject<ControllersResult>(await httpService.Get("https://10.114.113.101/api/application/night-city/modules/product/GetProductProcessList"));
                if (!result.Result)
                    throw new Exception(result.ErrorMessage);
                List<string> list = JsonConvert.DeserializeObject<List<string>>(result.Content.ToString());
                ProductProcessList = list;
            }
            catch (Exception e)
            {
                Global.Log($"[OnCall]:[MainViewModel]:[GetProductProcessListAsyncBack]:exception:{e.Message}", true);
            }
        }

        /// <summary>
        /// 查询集群
        /// </summary>
        /// <returns></returns>
        private async Task GetClustersAsyncBack()
        {
            try
            {
                await Task.Delay(MessageHost.InternalDelay);
                object mainboard = propertyService.GetProperty("Mainboard") ?? throw new Exception("Mainboard is null");
                ControllersResult result = JsonConvert.DeserializeObject<ControllersResult>(await httpService.Post("https://10.114.113.101/api/application/night-city/connection/GetClusters", new { Mainboard = mainboard.ToString() }));
                if (!result.Result)
                    throw new Exception(result.ErrorMessage);
                List<Connection_GetClusters_Result> list = JsonConvert.DeserializeObject<List<Connection_GetClusters_Result>>(result.Content.ToString());
                foreach (var cluster in list)
                {
                    if (cluster.Category == "Product")
                        SolveProductFromCluster = cluster.Cluster;
                    if (cluster.Category == "Process")
                        SolveProductProcessFromCluster = cluster.Cluster;
                }
            }
            catch (Exception e)
            {
                Global.Log($"[OnCall]:[MainViewModel]:[GetClustersAsyncBack]:exception:{e.Message}", true);
            }
        }

        /// <summary>
        /// 获取MQS测试Log
        /// </summary>
        /// <returns></returns>
        private async Task GetMQSLogAsyncBack()
        {
            try
            {
                await Task.Run(() =>
                {
                    string productProcess = string.Empty;
                    string product = string.Empty;
                    FileInfo[] files = new DirectoryInfo(@"D:\Lenovo log").GetFiles();
                    foreach (FileInfo file in files.OrderBy(it => it.CreationTime))
                    {
                        if (file.Extension != ".csv") continue;
                        StreamReader sr = new StreamReader(file.FullName, Encoding.UTF8);
                        string line = sr.ReadLine();
                        FileStream fs = new FileStream(file.FullName, FileMode.Open, FileAccess.Read);
                        int n = (int)fs.Length;
                        byte[] b = new byte[n];
                        int r = fs.Read(b, 0, n);
                        fs.Close();
                        string body = Encoding.UTF8.GetString(b, 0, n);
                        try
                        {
                            var rows = body.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                            product = rows[9].Substring(rows[9].IndexOf(",") + 1).Trim();
                            if (product.StartsWith("ANDROID_"))
                                product = product.Substring(product.IndexOf("_") + 1);
                            else if (product.StartsWith("TABLET_"))
                                product = product.Substring(product.IndexOf("_") + 1);
                            productProcess = rows[12].Substring(rows[12].IndexOf(",") + 1).Trim();
                        }
                        catch { }
                        if (ProductProcessList.Contains(productProcess) && ProductList.Contains(product))
                            goto Found;
                        else
                        {
                            product = string.Empty;
                            productProcess = string.Empty;
                        }
                    }
                Found:
                    if (productProcess != string.Empty && product != string.Empty)
                    {
                        SolveProductProcessFromFile = productProcess;
                        SolveProductFromFile = product;
                    }
                });
            }
            catch (Exception e)
            {
                Global.Log($"[OnCall]:[MainViewModel]:[GetMQSLogAsyncBack]:exception:{e.Message}", true);
            }
        }

        /// <summary>
        /// 获取Nextest测试Log
        /// </summary>
        /// <returns></returns>
        private async Task GetNextestLogAsyncBack()
        {
            try
            {
                await Task.Run(() =>
                {
                    string productProcess = string.Empty;
                    string product = string.Empty;
                    FileInfo[] files = new DirectoryInfo(@"C:\prod\log").GetFiles();
                    foreach (FileInfo file in files.OrderBy(it => it.CreationTime))
                    {
                        foreach (string pp in ProductProcessList)
                        {
                            foreach (string p in ProductList)
                            {
                                if (file.Name.StartsWith($"NexTestLogs_{pp}_{p}"))
                                {
                                    productProcess = pp;
                                    product = p;
                                    goto Found;
                                }
                            }
                        }
                    }
                Found:
                    if (productProcess != string.Empty && product != string.Empty)
                    {
                        SolveProductProcessFromFile = productProcess;
                        SolveProductFromFile = product;
                    }
                });
            }
            catch (Exception e)
            {
                Global.Log($"[OnCall]:[MainViewModel]:[GetNextestLogAsyncBack]:exception:{e.Message}", true);
            }
        }

        /// <summary>
        /// 添加待提交附件
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        private void AddPreCommittedAttachment(string filename)
        {
            try
            {
                AttachmentsError = string.Empty;
                FileInfo fileinfo = new FileInfo(filename);
                if (Attachments.FirstOrDefault(it => it.Name == fileinfo.Name) != null)
                    throw new Exception("Attachment with the same name");
                Attachment attachment = new Attachment
                {
                    Name = fileinfo.Name,
                    Extension = fileinfo.Extension,
                    Directory = fileinfo.DirectoryName,
                    Size = fileinfo.Length
                };
                long sizeTotal = 0;
                foreach (var at in Attachments)
                {
                    sizeTotal += at.Size;
                }
                sizeTotal += attachment.Size;
                if (sizeTotal > 15 * 1024 * 1024)
                    throw new Exception("The attachments is too large");
                FileStream stream = new FileStream(filename, FileMode.Open, FileAccess.Read);
                byte[] bytes = new byte[stream.Length];
                stream.Read(bytes, 0, bytes.Length);
                attachment.Base64Str = Convert.ToBase64String(bytes);
                stream.Close();
                Attachments.Add(attachment);
            }
            catch (Exception e)
            {
                Global.Log($"[OnCall]:[MainViewModel]:[AddPreCommittedAttachment]:exception:{e.Message}", true);
                AttachmentsError = e.Message;
            }
        }

        #region 命令集合

        #region 命令：同步横幅信息
        public ICommand SyncBannerMessagesCommand
        {
            get => new DelegateCommand(SyncBannerMessages);
        }
        private void SyncBannerMessages()
        {
            eventAggregator.GetEvent<BannerMessageSyncingEvent>().Publish();
        }
        #endregion

        #region 命令：同步进行中的异常报告
        public ICommand SyncOpenReportsCommand
        {
            get => new DelegateCommand(SyncOpenReports);
        }
        private async void SyncOpenReports()
        {
            await SyncOpenReportsAsync();
        }
        #endregion

        #region 命令：同步历史报告
        public ICommand GetAllReportsCommand
        {
            get => new DelegateCommand(GetAllReports);
        }
        private async void GetAllReports()
        {
            await GetAllReportsAsync();
        }
        #endregion

        #region 命令：生成异常报告EXCEL
        public ICommand ExportReportsToExcelCommand
        {
            get => new DelegateCommand(ExportReportsToExcel);
        }
        private async void ExportReportsToExcel()
        {
            await ExportReportsToExcelAsync(AllReportList);
        }
        #endregion

        #region 命令：切换视图
        public ICommand SwitchViewCommand
        {
            get => new DelegateCommand<string>(SwitchView);
        }
        private void SwitchView(string view)
        {
            View = view;
        }
        #endregion

        #region 命令：异常接单或填写解决回执
        public ICommand HandleReportCommand
        {
            get => new DelegateCommand<GetOpenReports_L2>(HandleReport);
        }
        private async void HandleReport(GetOpenReports_L2 report)
        {
            if (report.State == "triggered")
                await ResponseReportAsync(report.Id);
            else if (report.State == "responsed")
            {
                SolvedReportId = report.Id;
                TrySolveReport();
            }
        }

        #endregion

        #region 命令：异常解决回执询问
        public ICommand TrySolveReportCommand
        {
            get => new DelegateCommand(TrySolveReport);
        }
        private async void TrySolveReport()
        {
            SolvedProduct = null;
            SolvedProductProcess = string.Empty;
            SolvedFailureCategory = null;
            SolvedFailureReason = string.Empty;
            SolvedSolution = string.Empty;
            MessageHost.Show();
            MessageHost.DialogCategory = "Syncing";
            await GetProductListAsyncBack();
            await GetProductProcessListAsyncBack();
            await GetFailureCategoryListAsyncBack();
            await GetClustersAsyncBack();
            await GetMQSLogAsyncBack();
            await GetNextestLogAsyncBack();
            MessageHost.DialogCategory = "Solve Report";
        }
        #endregion

        #region 命令：异常解决
        public ICommand SolveReportCommand
        {
            get => new DelegateCommand(SolveReport);
        }
        private async void SolveReport()
        {
            await SolveReportAsync(SolvedReportId, SolvedProduct, SolvedProductProcess, SolvedFailureCategory, SolvedFailureReason, SolvedSolution);
        }
        #endregion

        #region 命令：异常误报
        public ICommand MisReportCommand
        {
            get => new DelegateCommand(MisReport);
        }
        private async void MisReport()
        {
            await MisReportAsync(SolvedReportId);
        }
        #endregion

        #region 命令：取消操作
        public ICommand CancelCommand
        {
            get => new DelegateCommand(Cancel);
        }
        public void Cancel()
        {
            MessageHost.HideImmediately();
        }
        #endregion

        #region 命令：填充异常解决回执
        public ICommand FillSolvedFieldCommand
        {
            get => new DelegateCommand<string>(FillSolvedField);
        }
        public void FillSolvedField(string field)
        {
            switch (field)
            {
                case "SolveProductFromFile":
                    SolvedProduct = SolveProductFromFile;
                    break;
                case "SolveProductFromCluster":
                    SolvedProduct = SolveProductFromCluster;
                    break;
                case "SolveProductProcessFromFile":
                    SolvedProductProcess = SolveProductProcessFromFile;
                    break;
                case "SolveProductProcessFromCluster":
                    SolvedProductProcess = SolveProductProcessFromCluster;
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region 命令：回执添加待提交附件
        public ICommand AddPreCommittedAttachmentCommand
        {
            get => new DelegateCommand(AddPreCommittedAttachment);
        }
        public void AddPreCommittedAttachment()
        {
            System.Windows.Forms.OpenFileDialog op = new System.Windows.Forms.OpenFileDialog
            {
                Title = "Please select the file to upload"
            };
            if (op.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                AddPreCommittedAttachment(op.FileName);
            }
        }
        #endregion

        #region 命令：回执取消待提交附件
        public ICommand RemovePreCommittedAttachmentCommand
        {
            get => new DelegateCommand<Attachment>(RemovePreCommittedAttachment);
        }
        public void RemovePreCommittedAttachment(Attachment attachment)
        {
            Attachments.Remove(attachment);
            AttachmentsError = string.Empty;
        }
        #endregion

        #endregion

        #region 可视化属性集合

        #region 视图类型
        private string view = "Dashboard";
        public string View
        {
            get => view;
            set
            {
                SetProperty(ref view, value);
            }
        }
        #endregion

        #region 异常解决回执：单号
        private string solvedReportId;
        public string SolvedReportId
        {
            get => solvedReportId;
            set
            {
                SetProperty(ref solvedReportId, value);
            }
        }
        #endregion

        #region 异常解决回执：产品
        private string solvedProduct;
        public string SolvedProduct
        {
            get => solvedProduct;
            set
            {
                SetProperty(ref solvedProduct, value);
            }
        }
        #endregion

        #region 异常解决回执：可能的产品 来源：文件
        private string solveProductFromFile;
        public string SolveProductFromFile
        {
            get => solveProductFromFile;
            set
            {
                SetProperty(ref solveProductFromFile, value);
            }
        }
        #endregion

        #region 异常解决回执：可能的产品 来源：集群
        private string solveProductFromCluster;
        public string SolveProductFromCluster
        {
            get => solveProductFromCluster;
            set
            {
                SetProperty(ref solveProductFromCluster, value);
            }
        }
        #endregion

        #region 异常解决回执：可能的流程 来源：文件
        private string solveProductProcessFromFile;
        public string SolveProductProcessFromFile
        {
            get => solveProductProcessFromFile;
            set
            {
                SetProperty(ref solveProductProcessFromFile, value);
            }
        }
        #endregion

        #region 异常解决回执：可能的流程 来源：集群
        private string solveProductProcessFromCluster;
        public string SolveProductProcessFromCluster
        {
            get => solveProductProcessFromCluster;
            set
            {
                SetProperty(ref solveProductProcessFromCluster, value);
            }
        }
        #endregion

        #region 异常解决回执：流程
        private string solvedProductProcess;
        public string SolvedProductProcess
        {
            get => solvedProductProcess;
            set
            {
                SetProperty(ref solvedProductProcess, value);
            }
        }
        #endregion

        #region 异常解决回执：故障类型
        private string solvedFailureCategory;
        public string SolvedFailureCategory
        {
            get => solvedFailureCategory;
            set
            {
                SetProperty(ref solvedFailureCategory, value);
            }
        }
        #endregion

        #region 异常解决回执：故障原因
        private string solvedFailureReason;
        public string SolvedFailureReason
        {
            get => solvedFailureReason;
            set
            {
                SetProperty(ref solvedFailureReason, value);
            }
        }
        #endregion

        #region 异常解决回执：解决方案
        private string solvedSolution;
        public string SolvedSolution
        {
            get => solvedSolution;
            set
            {
                SetProperty(ref solvedSolution, value);
            }
        }
        #endregion

        #region 产品列表
        private List<string> productList;
        public List<string> ProductList
        {
            get => productList;
            set
            {
                SetProperty(ref productList, value);
            }
        }
        #endregion

        #region 产品流程列表
        private List<string> productProcessList;
        public List<string> ProductProcessList
        {
            get => productProcessList;
            set
            {
                SetProperty(ref productProcessList, value);
            }
        }
        #endregion

        #region 故障类型列表
        private List<string> failureCategoryList;
        public List<string> FailureCategoryList
        {
            get => failureCategoryList;
            set
            {
                SetProperty(ref failureCategoryList, value);
            }
        }
        #endregion

        #region 本站点进行中异常报告列表
        private List<GetOpenReports_L2> localOpenReportList;
        public List<GetOpenReports_L2> LocalOpenReportList
        {
            get => localOpenReportList;
            set
            {
                SetProperty(ref localOpenReportList, value);
            }
        }
        #endregion

        #region 同辖站点进行中异常报告列表
        private List<GetOpenReports_L1> clusterOpenReportList;
        public List<GetOpenReports_L1> ClusterOpenReportList
        {
            get => clusterOpenReportList;
            set
            {
                SetProperty(ref clusterOpenReportList, value);
            }
        }
        #endregion

        #region 所有异常报告列表
        private List<GetAllReports_C1> allReportList;
        public List<GetAllReports_C1> AllReportList
        {
            get => allReportList;
            set
            {
                SetProperty(ref allReportList, value);
            }
        }
        #endregion

        #region 对话框设置
        private DialogSetting messageHost = new DialogSetting();
        public DialogSetting MessageHost
        {
            get => messageHost;
            set
            {
                SetProperty(ref messageHost, value);
            }
        }
        #endregion

        #region 待提交附件列表
        private ObservableCollection<Attachment> attachments = new ObservableCollection<Attachment>();
        public ObservableCollection<Attachment> Attachments
        {
            get => attachments;
            set
            {
                SetProperty(ref attachments, value);
            }
        }
        #endregion

        #region 待提交附件错误信息
        private string attachmentsError;
        public string AttachmentsError
        {
            get => attachmentsError;
            set
            {
                SetProperty(ref attachmentsError, value);
            }
        }
        #endregion

        #endregion

        #region P1
        private Func<double, string> _yFormatter1;
        public SeriesCollection SeriesCollection1 { get; set; }
        public Func<double, string> XFormatter1 { get; set; }
        public Func<double, string> YFormatter1
        {
            get { return _yFormatter1; }
            set
            {
                _yFormatter1 = value;
            }
        }
        #endregion

        #region P2
        public SeriesCollection SeriesCollection2 { get; set; }
        public string[] Labels2 { get; set; }
        public Func<double, string> Formatter2 { get; set; }
        #endregion

        #region P3
        public SeriesCollection SeriesCollection3 { get; set; }
        public string[] Labels3 { get; set; }
        public Func<double, string> YFormatter3 { get; set; }
        #endregion

        #region P4
        public Func<ChartPoint, string> PointLabel4 { get; set; }
        #endregion

        #region P5
        public SeriesCollection SeriesCollection5 { get; set; }
        private string[] _labels5;
        public string[] Labels5
        {
            get { return _labels5; }
            set
            {
                _labels5 = value;
            }
        }
        #endregion

        public void Dispose()
        {
            foreach (var eventToken in eventTokens)
            {
                eventToken.Dispose();
            }
            eventAggregator = null;
            propertyService = null;
            httpService = null;
        }
    }
}
