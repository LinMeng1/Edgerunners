using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moon.Attributes;
using Moon.Core.Models.Edgerunners;
using Moon.Core.Models;
using Moon.Core.Standard;
using Moon.Core.Utilities;

namespace Moon.Controllers.Application.MaxTac
{
    [Route("api/application/max-tac/user/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> log;
        public UserController(ILogger<UserController> _log)
        {
            log = _log;
        }

        #region 新增用户
        [Authorize]
        [PasswordCheck]
        [AuthorizeCheck(Authorization.AuthorizationEnum.Application_MaxTac_User_AddUser)]
        [HttpPost]
        public ControllersResult AddUser([FromBody] User_AddUser_Parameter parameter)
        {
            ControllersResult result = new();
            try
            {
                Users user = new()
                {
                    EmployeeId = parameter.EmployeeId,
                    ItCode = parameter.ItCode,
                    Password = Authentication.ToMD5(parameter.EmployeeId),
                    Name = parameter.Name,
                    Position = parameter.Position,
                    Email = parameter.Email,
                    Contact = parameter.Contact,
                };
                if (parameter.Organization != null)
                {
                    Organizations organization = Database.Edgerunners.Queryable<Organizations>().First(it => it.Id == parameter.Organization);
                    if (organization == null)
                        throw new Exception($"Invalid organization ({parameter.Organization}) , please refresh the page and try again");
                    user.Organization = parameter.Organization;
                }
                Database.Edgerunners.Insertable(user).ExecuteCommand();
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

        #region 修改用户信息
        [Authorize]
        [PasswordCheck]
        [AuthorizeCheck(Authorization.AuthorizationEnum.Application_MaxTac_User_SetUser)]
        [HttpPost]
        public ControllersResult SetUser([FromBody] User_SetUser_Parameter parameter)
        {
            ControllersResult result = new();
            try
            {
                Users user = Database.Edgerunners.Queryable<Users>().First(it => it.EmployeeId == parameter.EmployeeId);
                if (user == null)
                    throw new Exception($"Invalid exployeeId ({parameter.EmployeeId}) , please refresh the page and check");
                List<string> UpdateColumns = new();
                if (parameter.ItCode != null)
                {
                    user.ItCode = parameter.ItCode.Value;
                    UpdateColumns.Add("ItCode");
                }
                if (parameter.Name != null)
                {
                    user.Name = parameter.Name;
                    UpdateColumns.Add("Name");
                }
                if (parameter.Position != null)
                {
                    user.Position = parameter.Position;
                    UpdateColumns.Add("Position");
                }
                if (parameter.Email != null)
                {
                    user.Email = parameter.Email.Value;
                    UpdateColumns.Add("Email");
                }
                if (parameter.Contact != null)
                {
                    user.Contact = parameter.Contact.Value;
                    UpdateColumns.Add("Contact");
                }
                if (parameter.Organization != null)
                {
                    if (parameter.Organization.Value != null)
                    {
                        Organizations organization = Database.Edgerunners.Queryable<Organizations>().First(it => it.Id == parameter.Organization.Value);
                        if (organization == null)
                            throw new Exception($"Invalid organization ({parameter.Organization.Value}) , please refresh the page and try again");
                        if (Organization.IsBeManagementInfiniteLoop(parameter.EmployeeId, (int)parameter.Organization.Value))
                            throw new Exception($"Employee ({user.EmployeeId} is a manager with infinite loop organization )");
                    }
                    user.Organization = parameter.Organization.Value;
                    UpdateColumns.Add("Organization");
                }
                var update = Database.Edgerunners.Updateable(user).UpdateColumns(UpdateColumns.ToArray()).ExecuteCommand();
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

        #region 获取所有用户
        [HttpGet]
        public ControllersResult GetUsers()
        {
            ControllersResult result = new();
            try
            {
                result.Content = Database.Edgerunners.Queryable<Users>().LeftJoin<Organizations>((u, o) => u.Organization == o.Id)
                    .LeftJoin<Organizations>((u, o, o1) => u.EmployeeId == o1.Owner).Select((u, o, o1) => new
                    {
                        u.EmployeeId,
                        u.ItCode,
                        u.Name,
                        u.Position,
                        u.Email,
                        u.Contact,
                        u.Organization,
                        OrganizationName = o.Name,
                        ManageOrganization = (int?)(o1.Id == 0 ? null : o1.Id),
                        ManageOrganizationName = o1.Name,
                    }).ToList();
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

        #region 为用户增加角色
        [Authorize]
        [PasswordCheck]
        [AuthorizeCheck(Authorization.AuthorizationEnum.Application_MaxTac_User_AddRoleToUser)]
        [HttpPost]
        public ControllersResult AddRoleToUser([FromBody] User_AddRolesToUser_Parameter parameter)
        {
            ControllersResult result = new();
            try
            {
                Users_Roles users_Roles = new()
                {
                    UserEmployeeId = parameter.EmployeeId,
                    RoleId = parameter.Role,
                };
                Database.Edgerunners.Insertable(users_Roles).ExecuteCommand();
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

        #region 删除用户
        [Authorize]
        [PasswordCheck]
        [AuthorizeCheck(Authorization.AuthorizationEnum.Application_MaxTac_User_DeleteUser)]
        [HttpPost]
        public ControllersResult DeleteUser([FromBody] User_DeleteUser_Parameter parameter)
        {
            ControllersResult result = new();
            try
            {
                Users user = Database.Edgerunners.Queryable<Users>().First(it => it.EmployeeId == parameter.EmployeeId);
                if (user == null)
                    throw new Exception($"Invalid exployeeId ({parameter.EmployeeId}) , please refresh the page and check");
                Organizations organization = Database.Edgerunners.Queryable<Organizations>().First(it => it.Owner == user.EmployeeId);
                if (organization != null)
                {
                    List<Users> users = Database.Edgerunners.Queryable<Users>().Where(it => it.Organization == organization.Id).ToList();
                    if (users.Count > 0)
                        throw new Exception($"Cannot delete user ({user.EmployeeId}) , because it manages {users.Count} people , Please move them under someone else");
                    Database.Edgerunners.Deleteable<Organizations>().Where(it => it.Owner == user.EmployeeId).ExecuteCommand();
                }
                Database.Edgerunners.Deleteable(user).ExecuteCommand();
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



        #region AddUser
        public class User_AddUser_Parameter
        {
            public string EmployeeId { get; set; }
            public string? ItCode { get; set; }
            public string Name { get; set; }
            public string Position { get; set; }
            public string? Email { get; set; }
            public string? Contact { get; set; }
            public int? Organization { get; set; }
        }
        #endregion

        #region SetUser
        public class User_SetUser_Parameter
        {
            public string EmployeeId { get; set; }
            public Parameter_CanNullString? ItCode { get; set; }
            public string? Name { get; set; }
            public string? Position { get; set; }
            public Parameter_CanNullString? Email { get; set; }
            public Parameter_CanNullString? Contact { get; set; }
            public Parameter_CanNullInt? Organization { get; set; }
        }
        public class Parameter_CanNullString
        {
            public string? Value { get; set; }
        }
        public class Parameter_CanNullInt
        {
            public int? Value { get; set; }
        }
        #endregion

        #region AddRolesToUser
        public class User_AddRolesToUser_Parameter
        {
            public string EmployeeId { get; set; }
            public int Role { get; set; }
        }
        #endregion

        #region DeleteUser
        public class User_DeleteUser_Parameter
        {
            public string EmployeeId { get; set; }
        }
        #endregion
    }
}
