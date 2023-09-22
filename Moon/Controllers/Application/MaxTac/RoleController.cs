using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moon.Attributes;
using Moon.Core.Models.Edgerunners;
using Moon.Core.Models;
using Moon.Core.Standard;
using Moon.Core.Utilities;

namespace Moon.Controllers.Application.MaxTac
{
    [Route("api/application/max-tac/role/[action]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly ILogger<RoleController> log;
        public RoleController(ILogger<RoleController> _log)
        {
            log = _log;
        }

        #region 查询全角色权限表
        [Authorize]
        [HttpGet]
        public ControllersResult GetRolesAuthorizations()
        {
            ControllersResult result = new();
            try
            {
                List<Roles> roles = Database.Edgerunners.Queryable<Roles>().ToList();
                List<Roles_Authorizations> roles_Authorizations = Database.Edgerunners.Queryable<Roles_Authorizations>().ToList();
                List<object> list_roles_authorizations = new();
                foreach (Roles role in roles)
                {
                    List<string> authorizations = roles_Authorizations.Where(it => it.RoleId == role.Id).Select(it => ((Authorization.AuthorizationEnum)it.AuthorizationEnum).ToString()).ToList();
                    object role_authorizations = new
                    {
                        Role = role.Name,
                        Authorizations = authorizations
                    };
                    list_roles_authorizations.Add(role_authorizations);
                }
                result.Content = list_roles_authorizations;
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

        #region 新增角色
        [Authorize]
        [PasswordCheck]
        [AuthorizeCheck(Authorization.AuthorizationEnum.Application_MaxTac_Role_AddRole)]
        [HttpPost]
        public ControllersResult AddRole([FromBody] Organizations_AddRole_Parameter parameter)
        {
            ControllersResult result = new();
            try
            {
                Roles role = new()
                {
                    Name = parameter.Name,
                    Description = parameter.Description
                };
                int roleId = Database.Edgerunners.Insertable(role).ExecuteReturnIdentity();
                List<Roles_Authorizations> roles_Authorizations = new();
                foreach (int authorizationEnum in parameter.AuthorizationEnums)
                {
                    Roles_Authorizations roles_Authorization = new()
                    {
                        AuthorizationEnum = authorizationEnum,
                        RoleId = roleId
                    };
                    roles_Authorizations.Add(roles_Authorization);
                }
                Database.Edgerunners.Insertable(roles_Authorizations).IgnoreColumns("CreateTime").ExecuteCommand();
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

        #region 角色增加权限
        [Authorize]
        [PasswordCheck]
        [AuthorizeCheck(Authorization.AuthorizationEnum.Application_MaxTac_Role_AddAuthorizationToRole)]
        [HttpPost]
        public ControllersResult AddAuthorizationToRole(Role_AddAuthorizationToRole_Parameter parameter)
        {
            ControllersResult result = new();
            try
            {
                Roles role = Database.Edgerunners.Queryable<Roles>().First(it => it.Id == parameter.RoleId);
                if (role == null)
                    throw new Exception($"Invalid Role ({parameter.RoleId}) , please check and try again");
                if (!Enum.IsDefined(typeof(Authorization.AuthorizationEnum), parameter.AuthorizationEnum))
                    throw new Exception($"Invalid Authorization ({parameter.AuthorizationEnum}) , please check and try again");
                Roles_Authorizations roles_Authorizations = new()
                {
                    RoleId = role.Id,
                    AuthorizationEnum = parameter.AuthorizationEnum,
                };
                Database.Edgerunners.Insertable(roles_Authorizations).IgnoreColumns("CreateTime").ExecuteCommand();
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

        #region 角色删除权限
        [Authorize]
        [PasswordCheck]
        [AuthorizeCheck(Authorization.AuthorizationEnum.Application_MaxTac_Role_DeleteAuthorizationToRole)]
        [HttpPost]
        public ControllersResult DeleteAuthorizationToRole(Role_DeleteAuthorizationToRole_Parameter parameter)
        {
            ControllersResult result = new();
            try
            {
                Roles role = Database.Edgerunners.Queryable<Roles>().First(it => it.Id == parameter.RoleId);
                if (role == null)
                    throw new Exception($"Invalid Role ({parameter.RoleId}) , please check and try again");
                if (!Enum.IsDefined(typeof(Authorization.AuthorizationEnum), parameter.AuthorizationEnum))
                    throw new Exception($"Invalid Authorization ({parameter.AuthorizationEnum}) , please check and try again");
                Database.Edgerunners.Deleteable<Roles_Authorizations>().Where(it => it.RoleId == role.Id && it.AuthorizationEnum == parameter.AuthorizationEnum).ExecuteCommand();
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



        #region AddRoles
        public class Organizations_AddRole_Parameter
        {
            public string Name { get; set; }
            public string? Description { get; set; }
            public List<int> AuthorizationEnums { get; set; }
        }
        #endregion

        #region AddAuthorizationToRole
        public class Role_AddAuthorizationToRole_Parameter
        {
            public int RoleId { get; set; }
            public int AuthorizationEnum { get; set; }
        }
        #endregion

        #region DeleteAuthorizationToRole
        public class Role_DeleteAuthorizationToRole_Parameter
        {
            public int RoleId { get; set; }
            public int AuthorizationEnum { get; set; }
        }
        #endregion
    }
}
