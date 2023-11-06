using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moon.Attributes;
using Moon.Core.Models;
using Moon.Core.Models.Edgerunners;
using Moon.Core.Standard;
using Moon.Core.Utilities;

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
                Publish publish = Database.Edgerunners.Queryable<Publish>().OrderBy(it => it.Version, SqlSugar.OrderByType.Desc).First(it => it.Project == parameter.Project);
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
                Publish publish = new()
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
    }
}
