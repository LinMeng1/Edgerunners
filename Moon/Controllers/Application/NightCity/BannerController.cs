using Dm.Config;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moon.Attributes;
using Moon.Core.Models;
using Moon.Core.Models.Edgerunners;
using Moon.Core.Standard;
using Moon.Core.Utilities;
using static Moon.Controllers.Application.NightCity.BannerController;
using static Moon.Controllers.Application.NightCity.ConnectionController;

namespace Moon.Controllers.Application.NightCity
{
    [Route("api/application/night-city/banner/[action]")]
    [ApiController]
    public class BannerController : ControllerBase
    {
        private readonly ILogger<BannerController> log;
        public BannerController(ILogger<BannerController> _log)
        {
            log = _log;
        }

        #region 获取置顶信息
        [HttpPost]
        public ControllersResult GetMessages([FromBody] Banner_GetMessages_Parameter parameter)
        {
            ControllersResult result = new();
            try
            {
                List<Banner_GetMessages_Result> messages = Database.Edgerunners.Queryable<IPCBanners>().Where(it => it.Mainboard == parameter.Mainboard).OrderBy(it => it.CreateTime, SqlSugar.OrderByType.Desc).Select(it => new Banner_GetMessages_Result()
                {
                    Id = it.Id,
                    Urgency = it.Urgency,
                    Priority = it.Priority,
                    Category = it.Category,
                    Content = it.Content,
                    LinkCommand = it.LinkCommand,
                    LinkInfomation = it.LinkInformation,
                    Extensible = it.Extensible,
                    CreateTime = it.CreateTime,
                }).ToList();
                result.Content = messages;
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

        #region 设置置顶信息
        [HttpPost]
        public ControllersResult SetMessage([FromBody] Banner_SetMessage_Parameter parameter)
        {
            ControllersResult result = new();
            try
            {
                IPCBanners banners = new()
                {
                    Id = Guid.NewGuid().ToString(),
                    Mainboard = parameter.Mainboard,
                    Urgency = parameter.Urgency,
                    Priority = parameter.Priority,
                    Category = parameter.Category,
                    Content = parameter.Content,
                    LinkCommand = parameter.LinkCommand,
                    LinkInformation = parameter.LinkInfomation,
                    Extensible = parameter.Extensible,
                };
                Database.Edgerunners.Insertable(banners).IgnoreColumns("CreateTime").ExecuteCommand();
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

        #region 删除置顶信息
        [HttpPost]
        public ControllersResult RemoveMessage([FromBody] Banner_RemoveMessage_Parameter parameter)
        {
            ControllersResult result = new();
            try
            {
                Database.Edgerunners.Deleteable<IPCBanners>().Where(it => it.Id == parameter.Id).ExecuteCommand();
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

        #region GetMessages
        public class Banner_GetMessages_Parameter
        {
            public string Mainboard { get; set; }
        }
        public class Banner_GetMessages_Result
        {
            public string Id { get; set; }
            public string Urgency { get; set; }
            public int Priority { get; set; }
            public string Category { get; set; }
            public string Content { get; set; }
            public string LinkCommand { get; set; }
            public string LinkInfomation { get; set; }
            public bool Extensible { get; set; }
            public DateTime CreateTime { get; set; }
        }
        #endregion

        #region SetMessage
        public class Banner_SetMessage_Parameter
        {
            public string Mainboard { get; set; }
            public string Urgency { get; set; }
            public int Priority { get; set; }
            public string? Category { get; set; }
            public string Content { get; set; }
            public string? LinkCommand { get; set; }
            public string? LinkInfomation { get; set; }
            public bool Extensible { get; set; }
        }
        #endregion

        #region RemoveMessage
        public class Banner_RemoveMessage_Parameter
        {
            public string Id { get; set; }
        }
        #endregion

    }
}
