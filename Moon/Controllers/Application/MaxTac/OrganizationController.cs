using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moon.Attributes;
using Moon.Core.Models.Edgerunners;
using Moon.Core.Models;
using Moon.Core.Standard;
using Moon.Core.Utilities;

namespace Moon.Controllers.Application.MaxTac
{
    [Route("api/application/max-tac/organization/[action]")]
    [ApiController]
    public class OrganizationController : ControllerBase
    {
        private readonly ILogger<OrganizationController> log;
        public OrganizationController(ILogger<OrganizationController> _log)
        {
            log = _log;
        }

        #region 新增组织
        [Authorize]
        [PasswordCheck]
        [AuthorizeCheck(Authorization.AuthorizationEnum.Application_MaxTac_Role_AddAuthorizationToRole)]
        [HttpPost]
        public ControllersResult AddOrganization([FromBody] Organization_AddOrganization_Parameter parameter)
        {
            ControllersResult result = new();
            try
            {
                Users owner = Database.Edgerunners.Queryable<Users>().First(it => it.EmployeeId == parameter.Owner);
                if (owner == null)
                    throw new Exception($"Invalid organization owner ({parameter.Owner}) , please refresh the page and check");
                Organizations organization = new()
                {
                    Name = parameter.Name,
                    Owner = parameter.Owner,
                };
                int organizationId = Database.Edgerunners.Insertable(organization).ExecuteReturnIdentity();
                result.Content = new { organizationId };
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

        #region 修改组织
        [Authorize]
        [PasswordCheck]
        [AuthorizeCheck(Authorization.AuthorizationEnum.Application_MaxTac_Organization_SetOrganization)]
        [HttpPost]
        public ControllersResult SetOrganization([FromBody] Organization_SetOrganization_Parameter parameter)
        {
            ControllersResult result = new();
            try
            {
                if (parameter.Name == null && parameter.Owner == null)
                    throw new Exception($"One of Organization name and owner shouldnt be null");
                Organizations organization = new()
                {
                    Id = parameter.Id,
                    Name = parameter.Name,
                    Owner = parameter.Owner
                };
                if (parameter.Owner != null && Organization.IsManagementInfiniteLoop(parameter.Owner, parameter.Id))
                    throw new Exception($"Owner ({parameter.Owner} is a manager with infinite loop organization )");
                Database.Edgerunners.Updateable(organization).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommand();
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

        #region 查询组织架构
        [HttpGet]
        public ControllersResult GetOrganizationChart()
        {
            ControllersResult result = new();
            try
            {
                result.Content = Organization.GetOrganizationChartFlat();
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



        #region AddOrganization
        public class Organization_AddOrganization_Parameter
        {
            public string Name { get; set; }
            public string Owner { get; set; }
        }
        #endregion

        #region SetOrganization
        public class Organization_SetOrganization_Parameter
        {
            public int Id { get; set; }
            public string? Name { get; set; }
            public string? Owner { get; set; }
        }
        #endregion
    }
}
