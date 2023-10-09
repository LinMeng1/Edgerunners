using Dm.Config;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moon.Attributes;
using Moon.Core.Models;
using Moon.Core.Models.Edgerunners;
using Moon.Core.Standard;
using Moon.Core.Utilities;

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
                List<OnCall_CallRepor_Middleware> jurisdictionalClusters = Database.Edgerunners.Queryable<IPCs_JurisdictionalClusters>().RightJoin<IPCs_JurisdictionalClusters>((i, i2) => i.Owner == i2.Owner).Where((i, i2) => i.Mainboard == parameter.Mainboard).LeftJoin<Users>((i, i2, u) => i2.Owner == u.EmployeeId).Select((i, i2, u) => new OnCall_CallRepor_Middleware()
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
                            InitialOwner = cluster.Owner,
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
                result.Content = syncClusters;
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
                switch (report.State)
                {
                    case "triggered":
                        report.ResponseTime = DateTime.Now;
                        report.Responser = employeeID;
                        report.State = "responsed";
                        Database.Edgerunners.Updateable(report).UpdateColumns("ResponseTime", "Responser", "State").ExecuteCommand();
                        Database.Edgerunners.Updateable<IPCBanners>().SetColumns("Priority", 45).SetColumns("Content", $"{user.Name} is handling the issue").SetColumns("LinkCommand", $"module on-call solve issue {report.Id}").Where(it => it.Id == parameter.ReportId).ExecuteCommand();
                        Database.Edgerunners.Updateable<IPCBanners>().SetColumns("Priority", 15).SetColumns("Content", $"An opening issue report has been found on site:{report.HostName}. {user.Name} is handling the issue").Where(it => it.Id != parameter.ReportId && it.LinkInformation == parameter.ReportId).ExecuteCommand();
                        break;
                    case "responsed":
                        report.SolveTime = DateTime.Now;
                        report.Solver = employeeID;
                        report.State = "solved";
                        report.Product = parameter.Product;
                        report.Process = parameter.Process;
                        report.FailureCategory = parameter.FailureCategory;
                        report.FailureReason = parameter.FailureReason;
                        report.Solution = parameter.Solution;
                        Database.Edgerunners.Updateable(report).UpdateColumns("SolveTime", "Solver", "State", "Product", "Process", "FailureCategory", "FailureReason", "Solution").ExecuteCommand();
                        Database.Edgerunners.Deleteable<IPCBanners>().Where(it => it.LinkInformation == parameter.ReportId).ExecuteCommand();
                        break;
                    case "solved":
                        throw new Exception($"This issue has been solved, please refresh the banner message");
                    case "aborted":
                        throw new Exception($"This issue has been aborted, please refresh the banner message");
                    default:
                        throw new Exception($"Unexpected issue report state:{report.State}");
                }
                List<OnCall_CallRepor_Middleware> jurisdictionalClusters = Database.Edgerunners.Queryable<IPCs_JurisdictionalClusters>().RightJoin<IPCs_JurisdictionalClusters>((i, i2) => i.Owner == i2.Owner).Where((i, i2) => i.Mainboard == report.Mainboard).LeftJoin<Users>((i, i2, u) => i2.Owner == u.EmployeeId).Select((i, i2, u) => new OnCall_CallRepor_Middleware()
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
                result.Content = syncClusters;
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

        #region 查询当前报修
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

        #region CallReport
        public class OnCall_CallReport_Parameter
        {
            public string Mainboard { get; set; }
            public string HostName { get; set; }
        }
        public class OnCall_CallRepor_Middleware : IPCs_JurisdictionalClusters
        {
            public string OwnerName { get; set; }
        }      
        #endregion

        #region HandleReport
        public class OnCall_HandleReport_Parameter
        {
            public string ReportId { get; set; }
            public string? Product { get; set; }
            public string? Process { get; set; }
            public string? FailureCategory { get; set; }
            public string? FailureReason { get; set; }
            public string? Solution { get; set; }
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

    }
}
