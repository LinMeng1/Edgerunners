using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moon.Attributes;
using Moon.Core.Models.Edgerunners;
using Moon.Core.Models;
using Moon.Core.Standard;
using Moon.Core.Utilities;
using System.Text.RegularExpressions;
using Moon.Core.Models._Imaginary;

namespace Moon.Controllers.Basic
{
    [Route("api/basic/account/[action]")]
    [ApiController]
    [Authorize]
    [PasswordCheck]
    public class AccountController : ControllerBase
    {
        private readonly ILogger<AccountController> log;
        public AccountController(ILogger<AccountController> _log)
        {
            log = _log;
        }

        #region 修改本用户密码
        [HttpPost]
        public ControllersResult SetPassword([FromBody] Account_SetPassword_Parameter parameter)
        {
            ControllersResult result = new();
            try
            {
                string employeeID = Request.HttpContext.User.Claims.FirstOrDefault(it => it.Type == "EmployeeID").Value;
                string oldPassword = parameter.OldPassword;
                string newPassword = parameter.NewPassword;
                Users user = Database.Edgerunners.Queryable<Users>().First(it => it.EmployeeId == employeeID);
                if (user == null)
                    throw new Exception($"Invalid employeeId ({employeeID}) , please re login");
                else if (user.Password != Authentication.ToMD5(oldPassword))
                    throw new Exception("Invalid old password , please recheck");
                user.Password = Authentication.ToMD5(newPassword);
                Database.Edgerunners.Updateable(user).ExecuteCommand();
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

        #region 查询本用户角色及权限      
        [HttpGet]
        public ControllersResult GetRolesAndAuthorizations()
        {
            ControllersResult result = new();
            try
            {
                string employeeID = Request.HttpContext.User.Claims.FirstOrDefault(it => it.Type == "EmployeeID").Value;
                List<int> list_users_roles = Database.Edgerunners.Queryable<Users_Roles>().Where(it => it.UserEmployeeId == employeeID).Select(it => it.RoleId).ToList();
                List<Roles_Authorizations> list_roles_authorizations = Database.Edgerunners.Queryable<Roles_Authorizations>().Where(it => list_users_roles.Contains(it.RoleId)).ToList();
                List<Account_GetRolesAndAuthorizations_Result> results = new List<Account_GetRolesAndAuthorizations_Result>();
                foreach (int rolesId in list_users_roles)
                {
                    Roles role = Database.Edgerunners.Queryable<Roles>().First(it => it.Id == rolesId);
                    if (role != null)
                    {
                        List<int> list_authorizationEnum = list_roles_authorizations.Where(it => it.RoleId == rolesId).Select(it => it.AuthorizationEnum).ToList();
                        List<string> list_authorization = new();
                        foreach (int authorizationEnum in list_authorizationEnum)
                        {
                            list_authorization.Add(((Authorization.AuthorizationEnum)authorizationEnum).ToString());
                        }
                        results.Add(new Account_GetRolesAndAuthorizations_Result
                        {
                            Role = role.Name,
                            Authorizations = list_authorization
                        });
                    }
                }
                result.Content = results;
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

        #region 修改本用户联系方式
        [HttpPost]
        public ControllersResult SetContact([FromBody] Account_SetContact_Parameter parameter)
        {
            ControllersResult result = new();
            try
            {
                string pattern = @"^(13[0-9]|14[01456879]|15[0-35-9]|16[2567]|17[0-8]|18[0-9]|19[0-35-9])\d{8}$";
                Regex r = new(pattern, RegexOptions.IgnoreCase);
                Match m1 = r.Match(parameter.Contact);
                if (!m1.Success)
                    throw new Exception("Invalid phone number , please check and try again");
                string employeeID = Request.HttpContext.User.Claims.FirstOrDefault(it => it.Type == "EmployeeID").Value;
                Users user = Database.Edgerunners.Queryable<Users>().First(it => it.EmployeeId == employeeID);
                if (user == null)
                    throw new Exception($"Invalid employeeId ({employeeID}) , please re login");
                user.Contact = parameter.Contact;
                Database.Edgerunners.Updateable(user).ExecuteCommand();
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

        #region 查询本用户信息
        [HttpGet]
        public ControllersResult GetUserInformation()
        {
            ControllersResult result = new();
            try
            {
                string employeeID = Request.HttpContext.User.Claims.FirstOrDefault(it => it.Type == "EmployeeID").Value;
                Account_GetUserInformation_Result user = Database.Edgerunners.Queryable<Users>().LeftJoin<Organizations>((u, o) => u.Organization == o.Id).LeftJoin<Users_OfficePCs>((u, o, uo) => u.EmployeeId == uo.EmployeeId).Select((u, o, uo) => new Account_GetUserInformation_Result
                {
                    EmployeeId = u.EmployeeId,
                    ItCode = u.ItCode,
                    Name = u.Name,
                    Position = u.Position,
                    Email = u.Email,
                    Contact = u.Contact,
                    Organization = o.Name,
                    OfficeComputer = uo.Mainboard
                }).First(u => u.EmployeeId == employeeID);
                result.Content = user;
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

        #region 链接办公电脑
        [AuthorizeCheck(Authorization.AuthorizationEnum.Basic_Account_LinkOfficeComputer)]
        [HttpPost]
        public ControllersResult LinkOfficeComputer([FromBody] Account_LinkOfficeComputer_Parameter parameter)
        {
            ControllersResult result = new();
            try
            {
                string employeeID = Request.HttpContext.User.Claims.FirstOrDefault(it => it.Type == "EmployeeID").Value;
                Users_OfficePCs? officePC = Database.Edgerunners.Queryable<Users_OfficePCs>().First(it => it.EmployeeId == employeeID);
                if (officePC == null)
                    Database.Edgerunners.Insertable(new Users_OfficePCs()
                    {
                        EmployeeId = employeeID,
                        Mainboard = parameter.Mainboard,
                    }).IgnoreColumns("LinkTime").ExecuteCommand();
                else if (officePC.Mainboard != parameter.Mainboard)
                {
                    Database.Edgerunners.Updateable(new Users_OfficePCs()
                    {
                        EmployeeId = employeeID,
                        Mainboard = parameter.Mainboard,
                        LinkTime = DateTime.Now
                    }).ExecuteCommand();
                    Database.Edgerunners.Updateable<IPCBanners>().SetColumns(it => it.Mainboard == parameter.Mainboard).Where(it => it.Extensible && it.Mainboard == officePC.Mainboard).ExecuteCommand();
                }
                List<string> syncClusters = new() { parameter.Mainboard };
                if (officePC != null) syncClusters.Add(officePC.Mainboard);
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

        #region GetRolesAndAuthorizations
        public class Account_GetRolesAndAuthorizations_Result
        {
            public string Role { get; set; }
            public List<string> Authorizations { get; set; }
        }
        #endregion

        #region SetPassword
        public class Account_SetPassword_Parameter
        {
            public string OldPassword { get; set; }
            public string NewPassword { get; set; }
        }
        #endregion

        #region SetContact
        public class Account_SetContact_Parameter
        {
            public string Contact { get; set; }
        }
        #endregion

        #region SetOfficeComputer
        public class Account_SetOfficeComputer_Parameter
        {
            public string Mainboard { get; set; }
            public bool State { get; set; }
        }
        #endregion

        #region GetUserInformation
        public class Account_GetUserInformation_Result
        {
            public string EmployeeId { get; set; }
            public string? ItCode { get; set; }
            public string Name { get; set; }
            public string Position { get; set; }
            public string? Email { get; set; }
            public string? Contact { get; set; }
            public string? Organization { get; set; }
            public string? OfficeComputer { get; set; }
        }
        #endregion

        #region LinkOfficeComputer
        public class Account_LinkOfficeComputer_Parameter
        {
            public string Mainboard { get; set; }
        }
        #endregion
    }
}
