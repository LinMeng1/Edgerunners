using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moon.Attributes;
using Moon.Core.Models;
using Moon.Core.Models._Imaginary;
using Moon.Core.Models.Edgerunners;
using Moon.Core.Standard;
using Moon.Core.Utilities;
using Newtonsoft.Json;

namespace Moon.Controllers.Application.NightCity.Modules
{
    [Route("api/application/night-city/modules/on-call/[action]")]
    [ApiController]
    public class OnCallController : ControllerBase
    {
        private readonly ILogger<OnCallController> log;
        public OnCallController(ILogger<OnCallController> _log)
        {
            log = _log;
        }

        #region 异常报修
        [HttpPost]
        public ControllersResult CallReport([FromBody] OnCall_CallReport_Parameter parameter)
        {
            ControllersResult result = new();
            try
            {
                string id = Guid.NewGuid().ToString();
                int inProgressCount = Database.Edgerunners.Queryable<IPCIssueReports>().Where(it => it.Mainboard == parameter.Mainboard && it.State != "solved" && it.State != "aborted").Count();
                if (inProgressCount > 0) throw new Exception("Please do not report for repair repeatedly");
                List<OnCall_CallReport_Middleware> jurisdictionalClusters = Database.Edgerunners.Queryable<IPCs_JurisdictionalClusters>().RightJoin<IPCs_JurisdictionalClusters>((i, i2) => i.Owner == i2.Owner).Where((i, i2) => i.Mainboard == parameter.Mainboard).LeftJoin<Users>((i, i2, u) => i2.Owner == u.EmployeeId).Select((i, i2, u) => new OnCall_CallReport_Middleware()
                {
                    Mainboard = i2.Mainboard,
                    Category = i2.Category,
                    Cluster = i2.Cluster,
                    Owner = i2.Owner,
                    OwnerName = u.Name
                }).ToList();
                int index = 0;
                List<IPCBanners> banners = new();
                List<string> syncClusters = new();
                bool orderCreated = false;
                foreach (var cluster in jurisdictionalClusters)
                {
                    string syncCluster = $"{cluster.Category}/{cluster.Cluster}";
                    if (!syncClusters.Contains(syncCluster))
                        syncClusters.Add(syncCluster);
                    if (cluster.Mainboard == parameter.Mainboard)
                    {
                        IPCIssueReports report = new()
                        {
                            Id = id,
                            Mainboard = parameter.Mainboard,
                            HostName = parameter.HostName,
                            InitialOwner = cluster.Owner
                        };
                        IPCBanners banner = new()
                        {
                            Id = id,
                            Mainboard = parameter.Mainboard,
                            Urgency = "Execute",
                            Priority = 80,
                            Category = "OnCall",
                            Content = $"An issue has been found on this site. Please ask a test technician to handle it. Owner is {cluster.OwnerName ?? "Nobody"}",
                            LinkCommand = $"module on-call response issue {id}",
                            LinkInformation = id,
                        };
                        Database.Edgerunners.Insertable(report).InsertColumns("Id", "Mainboard", "HostName", "InitialOwner").ExecuteCommand();
                        orderCreated = true;
                        banners.Add(banner);
                    }
                    else
                    {
                        index++;
                        IPCBanners banner = new()
                        {
                            Id = $"{id}-{index}",
                            Mainboard = cluster.Mainboard,
                            Urgency = "Plan",
                            Priority = 80,
                            Category = "OnCall",
                            Content = $"An opening issue report has been found on site:{parameter.HostName}. Owner is {cluster.OwnerName ?? "Nobody"}",
                            LinkCommand = "module on-call sync issue",
                            LinkInformation = id,
                        };
                        banners.Add(banner);
                    }
                }
                Database.Edgerunners.Insertable(banners).IgnoreColumns("CreateTime").ExecuteCommand();
                if (!orderCreated)
                    throw new Exception("The order was not created successfully. Please check whether owner is displayed on the panel.");
                foreach (string syncCluster in syncClusters)
                {
                    try
                    {
                        _ = Mqtt.Publish(syncCluster, new _MqttMessage()
                        {
                            IsMastermind = true,
                            Address = "Moon",
                            Sender = "Lucy",
                            Content = "system sync banner messages"
                        });
                    }
                    catch { }
                }
                result.Result = true;
            }
            catch (Exception e)
            {
                result.ErrorMessage = $"Exception : {e.Message}";
                log.LogError(result.ErrorMessage);
            }
            return result;
        }
        #endregion

        #region 从Gigaeye异常报修
        [Authorize]
        [PasswordCheck]
        [AuthorizeCheck(Authorization.AuthorizationEnum.Application_NightCity_Modules_OnCall_CallReportFromGigaeye)]
        [HttpPost]
        public ControllersResult CallReportFromGigaeye([FromBody] OnCall_CallReportFromGigaeye_Paramter parameter)
        {
            ControllersResult result = new();
            try
            {
                string id = Guid.NewGuid().ToString();
                IPCs ipc = Database.Edgerunners.Queryable<IPCs>().First(it => it.HostName == parameter.HostName);
                if (ipc == null)
                    throw new Exception("Could not match computer entity");
                List<OnCall_CallReport_Middleware> jurisdictionalClusters = Database.Edgerunners.Queryable<IPCs_JurisdictionalClusters>().RightJoin<IPCs_JurisdictionalClusters>((i, i2) => i.Owner == i2.Owner).Where((i, i2) => i.Mainboard == ipc.Mainboard).LeftJoin<Users>((i, i2, u) => i2.Owner == u.EmployeeId).Select((i, i2, u) => new OnCall_CallReport_Middleware()
                {
                    Mainboard = i2.Mainboard,
                    Category = i2.Category,
                    Cluster = i2.Cluster,
                    Owner = i2.Owner,
                    OwnerName = u.Name
                }).ToList();
                int index = 0;
                List<IPCBanners> banners = new();
                List<string> syncClusters = new();
                bool orderCreated = false;
                foreach (var cluster in jurisdictionalClusters)
                {
                    string syncCluster = $"{cluster.Category}/{cluster.Cluster}";
                    if (!syncClusters.Contains(syncCluster))
                        syncClusters.Add(syncCluster);
                    if (cluster.Mainboard == ipc.Mainboard)
                    {
                        IPCIssueReports report = new()
                        {
                            Id = id,
                            Mainboard = ipc.Mainboard,
                            HostName = parameter.HostName,
                            Product = parameter.Product,
                            Process = parameter.Process,
                            FailureReason = $"[{parameter.Type}] at {parameter.Station}\r\n{parameter.TestCode}\r\n{parameter.Reason}",
                            InitialOwner = cluster.Owner,
                            ExternalId = parameter.Id,
                            ExternalSystem = "Gigaeye"
                        };
                        IPCBanners banner = new()
                        {
                            Id = id,
                            Mainboard = ipc.Mainboard,
                            Urgency = "Execute",
                            Priority = 90,
                            Category = "OnCall",
                            Content = $"[External:Gigaeye] TestCode：{parameter.TestCode}",
                            LinkCommand = $"module on-call response issue {id}",
                            LinkInformation = id,
                        };
                        Database.Edgerunners.Insertable(report).InsertColumns("Id", "Mainboard", "HostName", "InitialOwner", "Product", "Process", "FailureReason", "ExternalSystem", "ExternalId").ExecuteCommand();
                        orderCreated = true;
                        banners.Add(banner);
                    }
                    else
                    {
                        index++;
                        IPCBanners banner = new()
                        {
                            Id = $"{id}-{index}",
                            Mainboard = cluster.Mainboard,
                            Urgency = "Plan",
                            Priority = 90,
                            Category = "OnCall",
                            Content = $"[External:Gigaeye] An opening issue report has been found on site:{parameter.HostName}. Owner is {cluster.OwnerName ?? "Nobody"}",
                            LinkCommand = "module on-call sync issue",
                            LinkInformation = id,
                        };
                        banners.Add(banner);
                    }
                }
                Database.Edgerunners.Insertable(banners).IgnoreColumns("CreateTime").ExecuteCommand();
                if (!orderCreated)
                    throw new Exception("The order was not created successfully. Please check whether owner is displayed on the panel.");
                foreach (string syncCluster in syncClusters)
                {
                    try
                    {
                        _ = Mqtt.Publish(syncCluster, new _MqttMessage()
                        {
                            IsMastermind = true,
                            Address = "Moon",
                            Sender = "Lucy",
                            Content = "system sync banner messages"
                        });
                    }
                    catch { }
                }
                result.Result = true;
            }
            catch (Exception e)
            {
                result.ErrorMessage = $"Exception : {e.Message}";
                log.LogError(result.ErrorMessage);
            }
            return result;
        }
        #endregion

        #region 处理报修
        [Authorize]
        [PasswordCheck]
        [AuthorizeCheck(Authorization.AuthorizationEnum.Application_NightCity_Modules_OnCall_HandleReport)]
        [HttpPost]
        public ControllersResult HandleReport([FromBody] OnCall_HandleReport_Parameter parameter)
        {
            ControllersResult result = new();
            try
            {
                string employeeID = Request.HttpContext.User.Claims.FirstOrDefault(it => it.Type == "EmployeeID").Value;
                Users user = Database.Edgerunners.Queryable<Users>().First(it => it.EmployeeId == employeeID) ?? throw new Exception($"Invalid employeeId ({employeeID}) , please re login");
                IPCIssueReports report = Database.Edgerunners.Queryable<IPCIssueReports>().First(it => it.Id == parameter.ReportId) ?? throw new Exception($"Cant find issue report (id：{parameter.ReportId})");
                if (parameter.State != report.State) throw new Exception("Order state does not match server, Please try again later");
                string externalStr = report.ExternalSystem == null ? string.Empty : $"[External:{report.ExternalSystem}] ";
                switch (report.State)
                {
                    case "triggered":
                        report.ResponseTime = DateTime.Now;
                        report.Responser = employeeID;
                        report.State = "responsed";
                        Database.Edgerunners.Updateable(report).UpdateColumns("ResponseTime", "Responser", "State").ExecuteCommand();
                        Database.Edgerunners.Updateable<IPCBanners>().SetColumns("Priority", 40).SetColumns("Content", $"{externalStr}{user.Name} is handling the issue").SetColumns("LinkCommand", $"module on-call solve issue {report.Id}").Where(it => it.Id == parameter.ReportId).ExecuteCommand();
                        Database.Edgerunners.Updateable<IPCBanners>().SetColumns("Priority", 40).SetColumns("Content", $"{externalStr}An opening issue report has been found on site:{report.HostName}. {user.Name} is handling the issue").Where(it => it.Id != parameter.ReportId && it.LinkInformation == parameter.ReportId).ExecuteCommand();
                        switch (report.ExternalSystem)
                        {
                            case "Gigaeye":
                                _ = CloseGigaeyeOrderAsync(report);
                                break;
                            default:
                                break;
                        }
                        break;
                    case "responsed":
                        report.SolveTime = DateTime.Now;
                        report.Solver = employeeID;
                        if (parameter.AbortReason == null)
                        {
                            report.State = "solved";
                            report.Product = parameter.Product;
                            report.Process = parameter.Process;
                            report.FailureCategory = parameter.FailureCategory;
                            report.FailureReason = parameter.FailureReason;
                            report.Solution = parameter.Solution;
                        }
                        else
                        {
                            report.State = "aborted";
                            report.Solution = parameter.AbortReason;
                        }
                        Database.Edgerunners.Updateable(report).UpdateColumns("SolveTime", "Solver", "State", "Product", "Process", "FailureCategory", "FailureReason", "Solution").ExecuteCommand();
                        Database.Edgerunners.Deleteable<IPCBanners>().Where(it => it.LinkInformation == parameter.ReportId).ExecuteCommand();
                        switch (report.ExternalSystem)
                        {
                            case "Gigaeye":
                                _ = CloseGigaeyeOrderAsync(report);
                                break;
                            default:
                                break;
                        }
                        try
                        {
                            if (parameter.Attachments != null && parameter.Attachments.Count > 0)
                            {
                                string category = "TECC.NightCity.OnCall";
                                string subject = "OnCall Attachments";
                                string context = $"ID：{report.Id}<br/> " +
                                                 $"Computer：{report.HostName} ({report.Mainboard})<br/>" +
                                                 $"Product:{report.Product}<br/>" +
                                                 $"Process:{report.Process}<br/>" +
                                                 $"Sender：{user.Name}<br/>" +
                                                 $"FailureCategory:{report.FailureCategory}<br/>" +
                                                 $"FailureReason:{report.FailureReason}<br/>" +
                                                 $"Solution:{report.Solution}<br/>";
                                var receivers = Database.Edgerunners.Queryable<Products>().LeftJoin<Users>((p, u) => p.Engineer == u.EmployeeId).Where(p => p.InternalName == parameter.Product).Select((p, u) => u.Email).ToList();
                                Mail.Send(category, subject, context, receivers, new List<string>(), parameter.Attachments);
                            }
                        }
                        catch { }
                        break;
                    case "solved":
                        throw new Exception($"This issue has been solved, please refresh the banner message");
                    case "aborted":
                        throw new Exception($"This issue has been aborted, please refresh the banner message");
                    default:
                        throw new Exception($"Unexpected issue report state:{report.State}");
                }
                List<OnCall_CallReport_Middleware> jurisdictionalClusters = Database.Edgerunners.Queryable<IPCs_JurisdictionalClusters>().RightJoin<IPCs_JurisdictionalClusters>((i, i2) => i.Owner == i2.Owner).Where((i, i2) => i.Mainboard == report.Mainboard).LeftJoin<Users>((i, i2, u) => i2.Owner == u.EmployeeId).Select((i, i2, u) => new OnCall_CallReport_Middleware()
                {
                    Mainboard = i2.Mainboard,
                    Category = i2.Category,
                    Cluster = i2.Cluster,
                    Owner = i2.Owner,
                    OwnerName = u.Name
                }).ToList();
                List<string> syncClusters = new();
                foreach (var cluster in jurisdictionalClusters)
                {
                    string syncCluster = $"{cluster.Category}/{cluster.Cluster}";
                    if (!syncClusters.Contains(syncCluster))
                        syncClusters.Add(syncCluster);
                }
                foreach (string syncCluster in syncClusters)
                {
                    try
                    {
                        _ = Mqtt.Publish(syncCluster, new _MqttMessage()
                        {
                            IsMastermind = true,
                            Address = "Moon",
                            Sender = "Lucy",
                            Content = "system sync banner messages"
                        });
                    }
                    catch { }
                }
                result.Result = true;
            }
            catch (Exception e)
            {
                result.ErrorMessage = $"Exception : {e.Message}";
                log.LogError(result.ErrorMessage);
            }
            return result;
        }
        #endregion

        #region 获取订单详情
        [HttpPost]
        public ControllersResult GetReportDetails([FromBody] OnCall_GetReportDetails_Parameter parameter)
        {
            ControllersResult result = new();
            try
            {
                IPCIssueReports report = Database.Edgerunners.Queryable<IPCIssueReports>().First(it => it.Id == parameter.Id);
                result.Content = report;
                result.Result = true;
            }
            catch (Exception e)
            {
                result.ErrorMessage = $"Exception : {e.Message}";
                log.LogError(result.ErrorMessage);
            }
            return result;
        }
        #endregion

        #region 查询相关当前报修
        [HttpPost]
        public ControllersResult GetOpenReports([FromBody] OnCall_GetOpenReports_Parameter parameter)
        {
            ControllersResult result = new();
            try
            {
                List<OnCall_GetOpenReports_Result2> localRepairs = new();
                List<OnCall_GetOpenReports_Result1> clusterRepairs = new();
                List<IPCClusters> clusters = Database.Edgerunners.Queryable<IPCClusters>().Where(it => it.Mainboard == parameter.Mainboard && it.Category != null).ToList();
                foreach (IPCClusters cluster in clusters)
                {
                    OnCall_GetOpenReports_Result1 clusterRepair = new()
                    {
                        ClusterCategory = cluster.Category,
                        Cluster = cluster.Cluster,
                        Repairs = new List<OnCall_GetOpenReports_Result2>()
                    };
                    List<string> mainboards = Database.Edgerunners.Queryable<IPCClusters>().Where(it => it.Category == cluster.Category && it.Cluster == cluster.Cluster).Select(it => it.Mainboard).ToList();
                    List<IPCIssueReports> repairs = Database.Edgerunners.Queryable<IPCIssueReports>().Where(it => it.State != "solved" && it.State != "aborted" && mainboards.Any(s => s == it.Mainboard)).ToList();
                    foreach (IPCIssueReports repair in repairs)
                    {
                        if (repair.Mainboard == parameter.Mainboard)
                        {
                            if (localRepairs.FirstOrDefault(it => it.Id == repair.Id) == null)
                                localRepairs.Add(new()
                                {
                                    Id = repair.Id,
                                    Mainboard = repair.Mainboard,
                                    HostName = repair.HostName,
                                    State = repair.State,
                                    TriggerTime = repair.TriggerTime,
                                    ResponseTime = repair.ResponseTime,
                                });
                            continue;
                        }
                        clusterRepair.Repairs.Add(new()
                        {
                            Id = repair.Id,
                            Mainboard = repair.Mainboard,
                            HostName = repair.HostName,
                            State = repair.State,
                            TriggerTime = repair.TriggerTime,
                            ResponseTime = repair.ResponseTime,
                        });
                    }
                    clusterRepairs.Add(clusterRepair);
                }
                OnCall_GetOpenReports_Result1? locationRepairs = clusterRepairs.FirstOrDefault(it => it.ClusterCategory == "Location");
                OnCall_GetOpenReports_Result1? productRepairs = clusterRepairs.FirstOrDefault(it => it.ClusterCategory == "Product");
                if (productRepairs != null && locationRepairs != null)
                {
                    foreach (var repair in locationRepairs.Repairs)
                    {
                        var sameRepair = productRepairs.Repairs.FirstOrDefault(it => it.Id == repair.Id);
                        if (sameRepair != null)
                            productRepairs.Repairs.Remove(sameRepair);
                    }
                }
                clusterRepairs = clusterRepairs.Where(it => it.Repairs.Count > 0).ToList();
                result.Content = new OnCall_GetOpenReports_Result()
                {
                    LocalRepairs = localRepairs,
                    ClusterRepairs = clusterRepairs,
                };
                result.Result = true;
            }
            catch (Exception e)
            {
                result.ErrorMessage = $"Exception : {e.Message}";
                log.LogError(result.ErrorMessage);
            }
            return result;
        }
        #endregion

        #region 查询所有报修
        [HttpGet]
        public ControllersResult GetAllReports()
        {
            ControllersResult result = new();
            try
            {
                List<OnCall_GetAllReports_Result> reports = Database.Edgerunners.Queryable<IPCIssueReports>().OrderBy(it => it.TriggerTime, SqlSugar.OrderByType.Desc).Take(100)
                      .LeftJoin<Users>((i, u) => i.InitialOwner == u.EmployeeId)
                      .LeftJoin<Users>((i, u, u2) => i.TransferOwner == u2.EmployeeId)
                      .LeftJoin<Users>((i, u, u2, u3) => i.Responser == u3.EmployeeId)
                      .LeftJoin<Users>((i, u, u2, u3, u4) => i.Solver == u4.EmployeeId)
                      .Select((i, u, u2, u3, u4) => new OnCall_GetAllReports_Result()
                      {
                          Id = i.Id,
                          Mainboard = i.Mainboard,
                          HostName = i.HostName,
                          TriggerTime = i.TriggerTime,
                          InitialOwner = i.InitialOwner,
                          InitialOwnerName = u.Name,
                          TransferOwner = i.TransferOwner,
                          TransferOwnerName = u2.Name,
                          ResponseTime = i.ResponseTime,
                          Responser = i.Responser,
                          ResponserName = u3.Name,
                          SolveTime = i.SolveTime,
                          Solver = i.Solver,
                          SolverName = u4.Name,
                          State = i.State,
                          Product = i.Product,
                          Process = i.Process,
                          FailureCategory = i.FailureCategory,
                          FailureReason = i.FailureReason,
                          Solution = i.Solution,
                          ExternalSystem = i.ExternalSystem,
                          ExternalId = i.ExternalId,
                      }).ToList();
                result.Content = reports;
                result.Result = true;
            }
            catch (Exception e)
            {
                result.ErrorMessage = $"Exception : {e.Message}";
                log.LogError(result.ErrorMessage);
            }
            return result;
        }
        #endregion

        #region 查询故障类型列表
        [HttpGet]
        public ControllersResult GetFailureCategoryList()
        {
            ControllersResult result = new();
            try
            {
                List<string> failureCategories = Database.Edgerunners.Queryable<IPCIssueFailureCategories>().OrderBy(it => it.Id).Select(it => it.Name).ToList();
                result.Content = failureCategories;
                result.Result = true;
            }
            catch (Exception e)
            {
                result.ErrorMessage = $"Exception : {e.Message}";
                log.LogError(result.ErrorMessage);
            }
            return result;
        }
        #endregion

        #region 查询分配信息列表
        [HttpGet]
        public ControllersResult GetAssignInfoList()
        {
            ControllersResult result = new();
            try
            {
                string employeeID = string.Empty;
                try
                {
                    employeeID = Request.HttpContext.User.Claims.FirstOrDefault(it => it.Type == "EmployeeID").Value;
                }
                catch { }
                List<OnCall_GetAssignInfoList_Result> assignInfoList = Database.Edgerunners.Queryable<IPCClusters_Owners>().Where(it => it.Category == "Location" || it.Category == "Product").Select(it => new OnCall_GetAssignInfoList_Result()
                {
                    Category = it.Category,
                    Cluster = it.Cluster,
                    Creator = it.Creator,
                    Owner = it.Owner,
                }).ToList();
                foreach (var assignInfo in assignInfoList)
                {
                    if (employeeID != null && employeeID == assignInfo.Creator)
                        assignInfo.IsControllable = true;
                }
                result.Content = assignInfoList;
                result.Result = true;
            }
            catch (Exception e)
            {
                result.ErrorMessage = $"Exception : {e.Message}";
                log.LogError(result.ErrorMessage);
            }
            return result;
        }
        #endregion

        #region 查询故障原因列表
        [HttpPost]
        public ControllersResult GetFailureReasonList([FromBody] OnCall_GetFailureReasonList_Parameter parameter)
        {
            ControllersResult result = new();
            try
            {
                List<string> failureReasonList = Database.Edgerunners.Queryable<ProductProcesses_FailureReasons>().Where(it => it.ProductProcess == parameter.Process).Select(it => it.FailureReason).ToList();
                result.Content = failureReasonList;
                result.Result = true;
            }
            catch (Exception e)
            {
                result.ErrorMessage = $"Exception : {e.Message}";
                log.LogError(result.ErrorMessage);
            }
            return result;
        }
        #endregion

        #region 查询解决方案列表
        [HttpPost]
        public ControllersResult GetSolutionList([FromBody] OnCall_GetSolutionList_Parameter parameter)
        {
            ControllersResult result = new();
            try
            {
                List<string> failureReasonList = Database.Edgerunners.Queryable<ProductProcesses_Solutions>().Where(it => it.ProductProcess == parameter.Process).Select(it => it.Solution).ToList();
                result.Content = failureReasonList;
                result.Result = true;
            }
            catch (Exception e)
            {
                result.ErrorMessage = $"Exception : {e.Message}";
                log.LogError(result.ErrorMessage);
            }
            return result;
        }
        #endregion


        #region CallReport
        public class OnCall_CallReport_Parameter
        {
            public string Mainboard { get; set; }
            public string HostName { get; set; }
        }
        public class OnCall_CallReport_Middleware : IPCs_JurisdictionalClusters
        {
            public string OwnerName { get; set; }
        }
        #endregion

        #region CallReportFromGigaeye
        public class OnCall_CallReportFromGigaeye_Paramter
        {
            public string Id { get; set; }
            public string HostName { get; set; }
            public string Product { get; set; }
            public string Process { get; set; }
            public string TestCode { get; set; }
            public string Type { get; set; }
            public string Station { get; set; }
            public string Reason { get; set; }
        }
        private static async Task CloseGigaeyeOrderAsync(IPCIssueReports report)
        {
            string destination = string.Empty;
            object parameter = null;
            Dictionary<string, string> headers = null;
            try
            {
                destination = "https://datashark.lenovo.com/api/sys/auth";
                parameter = new { username = "guard", password = "WpJM-2178" };
                DatasharkAPIResult auth = JsonConvert.DeserializeObject<DatasharkAPIResult>(await Http.Post(destination, parameter));
                if (auth.code != 200) throw new Exception($"{auth}");
                headers = new Dictionary<string, string>
                {
                    { "X-Access-Token", auth.result }
                };
                destination = "https://datashark.lenovo.com/api/keyItemWarn/updateStatus";
                parameter = new
                {
                    gigaeyeID = report.ExternalId,
                    mainboard = report.Mainboard,
                    state = report.State,
                    owner = string.Empty,
                    solver = string.Empty,
                    solution = report.Solution,
                    responseTime = report.ResponseTime,
                    solveTime = report.SolveTime,
                };
                DatasharkAPIResult updateStatus = JsonConvert.DeserializeObject<DatasharkAPIResult>(await Http.Post(destination, parameter, headers));
                if (updateStatus.code != 200) throw new Exception(JsonConvert.SerializeObject(updateStatus));
            }
            catch (Exception e)
            {
                Database.Edgerunners.Insertable(new LegacyInformations()
                {
                    Id = Guid.NewGuid().ToString(),
                    Channel = "Http.Post",
                    Destination = destination,
                    Information = JsonConvert.SerializeObject(parameter),
                    FeedBack = e.Message,
                    Addition = headers == null ? null : JsonConvert.SerializeObject(headers),
                }).IgnoreColumns("Time").ExecuteCommand();
            }
        }
        private class DatasharkAPIResult
        {
            public bool success { get; set; }
            public string message { get; set; }
            public int code { get; set; }
            public string result { get; set; }
            public long timestamp { get; set; }
        }
        #endregion

        #region HandleReport
        public class OnCall_HandleReport_Parameter
        {
            public string ReportId { get; set; }
            public string State { get; set; }
            public string? Product { get; set; }
            public string? Process { get; set; }
            public string? FailureCategory { get; set; }
            public string? FailureReason { get; set; }
            public string? Solution { get; set; }
            public string? AbortReason { get; set; }
            public List<_Attachment>? Attachments { get; set; }
        }
        #endregion

        #region GetReportDetails
        public class OnCall_GetReportDetails_Parameter
        {
            public string Id { get; set; }
        }
        #endregion

        #region GetOpenReports
        public class OnCall_GetOpenReports_Parameter
        {
            public string Mainboard { get; set; }
        }
        public class OnCall_GetOpenReports_Result
        {
            public List<OnCall_GetOpenReports_Result2> LocalRepairs { get; set; }
            public List<OnCall_GetOpenReports_Result1> ClusterRepairs { get; set; }
        }
        public class OnCall_GetOpenReports_Result1
        {
            public string Cluster { get; set; }
            public string ClusterCategory { get; set; }
            public List<OnCall_GetOpenReports_Result2> Repairs { get; set; }
        }
        public class OnCall_GetOpenReports_Result2
        {
            public string Id { get; set; }
            public string Mainboard { get; set; }
            public string HostName { get; set; }
            public string State { get; set; }
            public DateTime TriggerTime { get; set; }
            public DateTime? ResponseTime { get; set; }
        }
        #endregion

        #region GetAllReports
        public class OnCall_GetAllReports_Result : IPCIssueReports
        {
            public string? InitialOwnerName { get; set; }
            public string? TransferOwnerName { get; set; }
            public string? ResponserName { get; set; }
            public string? SolverName { get; set; }
        }
        #endregion

        #region GetAssignInfoList
        public class OnCall_GetAssignInfoList_Result : IPCClusters_Owners
        {
            public bool IsControllable { get; set; }
        }
        #endregion

        #region GetFailureReasonList
        public class OnCall_GetFailureReasonList_Parameter
        {
            public string Process { get; set; }
        }
        #endregion

        #region GetSolutionList
        public class OnCall_GetSolutionList_Parameter
        {
            public string Process { get; set; }
        }
        #endregion

    }
}
