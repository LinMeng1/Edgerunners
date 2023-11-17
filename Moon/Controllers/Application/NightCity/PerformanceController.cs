using Microsoft.AspNetCore.Mvc;
using Moon.Core.Models;
using Moon.Core.Models.Edgerunners;
using Moon.Core.Standard;

namespace Moon.Controllers.Application.NightCity
{
    [Route("api/application/night-city/performance/[action]")]
    [ApiController]
    public class PerformanceController : ControllerBase
    {
        private readonly ILogger<PerformanceController> log;
        public PerformanceController(ILogger<PerformanceController> _log)
        {
            log = _log;
        }

        #region 上传本机基础信息
        [HttpPost]
        public ControllersResult UploadBasicInformation([FromBody] Performance_UploadBasicInformation_Parameter parameter)
        {
            ControllersResult result = new();
            try
            {
                IPCHistories history = new()
                {
                    Mainboard = parameter.Mainboard,
                    HostName = parameter.HostName,
                    HostMac = parameter.HostMac,
                    HostAddress = parameter.HostAddress,
                    Domain = parameter.Domain,
                    Cpu = parameter.Cpu,
                    Disk = parameter.Disk,
                    Memory = parameter.Memory,
                };
                Database.Edgerunners.Insertable(history).IgnoreColumns("UploadTime").ExecuteCommand();
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

        #region 上传本机NightCity版本
        [HttpPost]
        public ControllersResult UploadNightCityVersion([FromBody] Performance_UploadNightCityVersion_Parameter parameter)
        {
            ControllersResult result = new();
            try
            {
                NightCityLaunchHistories history = new()
                {
                    Mainboard = parameter.Mainboard,
                    Version = parameter.Version,
                };
                Database.Edgerunners.Insertable(history).IgnoreColumns("Id").IgnoreColumns("Time").ExecuteCommand();
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

        #region UploadBasicInformation
        public class Performance_UploadBasicInformation_Parameter
        {
            public string Mainboard { get; set; }
            public string? HostName { get; set; }
            public string? HostMac { get; set; }
            public string? HostAddress { get; set; }
            public string? Domain { get; set; }
            public string? Cpu { get; set; }
            public string? Disk { get; set; }
            public string? Memory { get; set; }
        }
        #endregion

        #region UploadNightCityVersion
        public class Performance_UploadNightCityVersion_Parameter
        {
            public string Mainboard { get; set; }
            public string Version { get; set; }
        }
        #endregion 
    }
}
