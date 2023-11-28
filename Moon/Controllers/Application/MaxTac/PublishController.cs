using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moon.Attributes;
using Moon.Core.Models;
using Moon.Core.Models.Edgerunners;
using Moon.Core.Standard;
using Moon.Core.Utilities;
using System.Security.Policy;
using System.Text.RegularExpressions;

namespace Moon.Controllers.Application.MaxTac
{
    [Route("api/application/max-tac/publish/[action]")]
    [ApiController]
    public class PublishController : ControllerBase
    {
        private readonly ILogger<PublishController> log;
        public PublishController(ILogger<PublishController> _log)
        {
            log = _log;
        }

        #region 查询开发者新闻
        [HttpGet]
        public ControllersResult GetDeveloperNews()
        {
            ControllersResult result = new();
            try
            {
                result.Content = Database.Edgerunners.Queryable<DeveloperNews>().OrderBy(it => it.Time, SqlSugar.OrderByType.Desc).Take(10).ToList();
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

        #region 查询最新发布
        [HttpPost]
        public ControllersResult GetLatestRelease([FromBody] Publish_GetLatestRelease_Parameter parameter)
        {
            ControllersResult result = new();
            try
            {
                Publications publish = Database.Edgerunners.Queryable<Publications>().OrderBy(it => it.Version, SqlSugar.OrderByType.Desc).First(it => it.Project == parameter.Project);
                if (publish == null) throw new Exception($"No release information for the Project ({parameter.Project}) found");
                result.Content = publish;
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

        #region 发布项目
        [Authorize]
        [PasswordCheck]
        [AuthorizeCheck(Authorization.AuthorizationEnum.Application_MaxTac_Publish_ReleaseProject)]
        [HttpPost]
        public ControllersResult ReleaseProject([FromBody] Publish_ReleaseProject_Parameter parameter)
        {
            ControllersResult result = new();
            try
            {
                Publications publish = new()
                {
                    Project = parameter.Project,
                    Version = parameter.Version,
                    ReleaseNotes = parameter.ReleaseNotes,
                    Manifest = Manifest.GetManifest(parameter.ReleaseAddress),
                    ReleaseAddress = parameter.ReleaseAddress
                };
                var storage = Database.Edgerunners.Storageable(publish).ToStorage();
                storage.AsInsertable.IgnoreColumns("ReleaseTime").ExecuteCommand();
                storage.AsUpdateable.IgnoreColumns("ReleaseTime").ExecuteCommand();
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

        #region 更新模块
        [Authorize]
        [PasswordCheck]
        [AuthorizeCheck(Authorization.AuthorizationEnum.Application_MaxTac_Publish_UpdateNightCityModule)]
        [HttpPost]
        public ControllersResult UpdateNightCityModule([FromBody] Publish_UpdateNightCityModule_Parameter parameter)
        {
            ControllersResult result = new();
            try
            {
                Modules module = Database.Edgerunners.Queryable<Modules>().First(it => it.Name == parameter.Module);
                if (module == null) throw new Exception("Invalid module name");
                string pattern = @"([0-9]*)\.([0-9]*)\.([0-9]*)\.([0-9]*)";
                Regex r = new(pattern, RegexOptions.IgnoreCase);
                Match m1 = r.Match(parameter.Version);
                if (!m1.Success)
                    throw new Exception("Invalid version , please check and try again");
                Modules_Versions version = new()
                {
                    ModuleId = module.Id,
                    Version = parameter.Version,
                    ReleaseNotes = parameter.ReleaseNotes,
                    Manifest = Manifest.GetManifest($"/NightCity.Modules/{parameter.Module}/{parameter.Version}"),
                };
                var storage = Database.Edgerunners.Storageable(version).ToStorage();
                storage.AsInsertable.IgnoreColumns("ReleaseTime").ExecuteCommand();
                storage.AsUpdateable.IgnoreColumns("ReleaseTime").ExecuteCommand();
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

        #region GetLatestRelease
        public class Publish_GetLatestRelease_Parameter
        {
            public string Project { get; set; }
        }
        #endregion

        #region ReleaseProject
        public class Publish_ReleaseProject_Parameter
        {
            public string Project { get; set; }
            public string Version { get; set; }
            public string ReleaseNotes { get; set; }
            public string ReleaseAddress { get; set; }
        }
        #endregion

        #region UpdateNightCityModule 
        public class Publish_UpdateNightCityModule_Parameter
        {
            public string Module { get; set; }
            public string Version { get; set; }
            public string ReleaseNotes { get; set; }
        }
        #endregion
    }
}
