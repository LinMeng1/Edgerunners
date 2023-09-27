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
        public ControllersResult CallRepair([FromBody] OnCall_CallRepair_Parameter parameter)
        {
            ControllersResult result = new();
            try
            {
                string id = Guid.NewGuid().ToString();
                int inProgressCount = Database.Edgerunners.Queryable<IPCIssueReports>().Where(it => it.Mainboard == parameter.Mainboard && it.State != "solved" && it.State != "aborted").Count();
                if (inProgressCount > 0) throw new Exception("Please do not report for repair repeatedly");
                IPCIssueReports report = new()
                {
                    Id = id,
                    Mainboard = parameter.Mainboard,
                    HostName = parameter.HostName,
                    InitialOwner = parameter.OwnerEmployeeId,
                };
                Database.Edgerunners.Insertable(report).InsertColumns("Id", "Mainboard", "HostName", "InitialOwner").ExecuteCommand();
                IPCBanners banner = new()
                {
                    Id = id,
                    Mainboard = parameter.Mainboard,
                    Urgency = "Execute",
                    Priority = 80,
                    Category = "OnCall",
                    Content = $"An issue has been found on this site. Please ask a test technician to handle it. Owner is {parameter.Owner ?? "Nobody"}",
                    LinkCommand = $"module on-call response issue {id}",
                    LinkInformation = id,
                };
                Database.Edgerunners.Insertable(banner).IgnoreColumns("CreateTime").ExecuteCommand();
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
        [AuthorizeCheck(Authorization.AuthorizationEnum.Application_NightCity_Modules_OnCall_HandleRepair)]
        [HttpPost]
        public ControllersResult HandleRepair([FromBody] OnCall_HandleRepair_Parameter parameter)
        {
            ControllersResult result = new();
            try
            {
                string employeeID = Request.HttpContext.User.Claims.FirstOrDefault(it => it.Type == "EmployeeID").Value;
                Users user = Database.Edgerunners.Queryable<Users>().First(it => it.EmployeeId == employeeID);
                if (user == null) throw new Exception($"Invalid employeeId ({employeeID}) , please re login");
                IPCIssueReports report = Database.Edgerunners.Queryable<IPCIssueReports>().First(it => it.Id == parameter.ReportId);
                if (report == null) throw new Exception($"Cant find issue report (id：{parameter.ReportId})");
                switch (report.State)
                {
                    case "triggered":
                        report.ResponseTime = DateTime.Now;
                        report.Responser = employeeID;
                        report.State = "responsed";
                        Database.Edgerunners.Updateable(report).UpdateColumns("ResponseTime", "Responser", "State").ExecuteCommand();
                        Database.Edgerunners.Updateable<IPCBanners>().SetColumns("Priority", 45).SetColumns("Content", $"{user.Name} is handling the issue").SetColumns("LinkCommand", $"module on-call solve issue {report.Id}").Where(it => it.Id == parameter.ReportId).ExecuteCommand();
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
                        Database.Edgerunners.Deleteable<IPCBanners>().Where(it => it.Id == parameter.ReportId).ExecuteCommand();
                        break;
                    case "solved":
                        throw new Exception($"This issue has been solved, please refresh the banner message");
                    case "aborted":
                        throw new Exception($"This issue has been aborted, please refresh the banner message");
                    default:
                        throw new Exception($"Unexpected issue report state:{report.State}");
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

        #region 查询当前报修
        [HttpPost]
        public ControllersResult GetRepairs([FromBody] OnCall_GetRepairs_Parameter parameter)
        {
            ControllersResult result = new();
            try
            {
                List<OnCall_GetRepairs_Result2> localRepairs = new();
                List<OnCall_GetRepairs_Result> clusterRepairs = new();
                List<IPCClusters> clusters = Database.Edgerunners.Queryable<IPCClusters>().Where(it => it.Mainboard == parameter.Mainboard && it.Category != null).ToList();
                foreach (IPCClusters cluster in clusters)
                {
                    OnCall_GetRepairs_Result clusterRepair = new();
                    clusterRepair.ClusterCategory = cluster.Category;
                    clusterRepair.Cluster = cluster.Cluster;
                    clusterRepair.Repairs = new List<OnCall_GetRepairs_Result2>();
                    List<string> mainboards = Database.Edgerunners.Queryable<IPCClusters>().Where(it => it.Category == cluster.Category && it.Cluster == cluster.Cluster).Select(it => it.Mainboard).ToList();
                    List<IPCIssueReports> repairs = Database.Edgerunners.Queryable<IPCIssueReports>().Where(it => it.State != "solved" && it.State != "aborted" && mainboards.Any(s => s == it.Mainboard)).ToList();
                    foreach (IPCIssueReports repair in repairs)
                    {
                        if (repair.Mainboard == parameter.Mainboard && localRepairs.FirstOrDefault(it => it.Id == repair.Id) == null)
                        {
                            localRepairs.Add(new()
                            {
                                Id = repair.Id,
                                Mainboard = repair.Mainboard,
                                HostName = repair.HostName,
                                State = repair.State,
                            });
                        }
                        clusterRepair.Repairs.Add(new()
                        {
                            Id = repair.Id,
                            Mainboard = repair.Mainboard,
                            HostName = repair.HostName,
                            State = repair.State,
                        });
                    }
                    clusterRepairs.Add(clusterRepair);
                }
                OnCall_GetRepairs_Result? locationRepairs = clusterRepairs.FirstOrDefault(it => it.ClusterCategory == "Location");
                OnCall_GetRepairs_Result? productRepairs = clusterRepairs.FirstOrDefault(it => it.ClusterCategory == "Product");
                if (productRepairs != null && locationRepairs != null)
                {
                    foreach (var repair in locationRepairs.Repairs)
                    {
                        var sameRepair = productRepairs.Repairs.FirstOrDefault(it => it.Id == repair.Id);
                        if (sameRepair != null)
                            productRepairs.Repairs.Remove(sameRepair);
                    }
                }
                result.Content = new
                {
                    localRepairs,
                    clusterRepairs
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

        #region CallRepair
        public class OnCall_CallRepair_Parameter
        {
            public string Mainboard { get; set; }
            public string HostName { get; set; }
            public string? Owner { get; set; }
            public string? OwnerEmployeeId { get; set; }
        }
        #endregion

        #region HandleRepair
        public class OnCall_HandleRepair_Parameter
        {
            public string ReportId { get; set; }
            public string? Product { get; set; }
            public string? Process { get; set; }
            public string? FailureCategory { get; set; }
            public string? FailureReason { get; set; }
            public string? Solution { get; set; }
        }
        #endregion

        #region GetRepairs
        public class OnCall_GetRepairs_Parameter
        {
            public string Mainboard { get; set; }
        }
        public class OnCall_GetRepairs_Result
        {
            public string Cluster { get; set; }
            public string ClusterCategory { get; set; }
            public List<OnCall_GetRepairs_Result2> Repairs { get; set; }
        }
        public class OnCall_GetRepairs_Result2
        {
            public string Id { get; set; }
            public string Mainboard { get; set; }
            public string HostName { get; set; }
            public string State { get; set; }
        }
        #endregion
    }
}
