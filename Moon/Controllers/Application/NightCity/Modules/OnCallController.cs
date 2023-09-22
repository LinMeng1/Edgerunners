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
                    LinkCommand = $"module on-call handle issue {id}",
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
                        Database.Edgerunners.Updateable<IPCBanners>().SetColumns(it => it.Content == $"{user.Name} is handling the issue").Where(it => it.Id == parameter.ReportId).ExecuteCommand();
                        break;
                    case "responsed":
                        throw new Exception($"Unexpected issue report state:{report.State}");
                        break;
                    case "solved":
                        throw new Exception($"Unexpected issue report state:{report.State}");
                        break;
                    case "aborted":
                        throw new Exception($"Unexpected issue report state:{report.State}");
                        break;
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
        }
        #endregion
    }
}
