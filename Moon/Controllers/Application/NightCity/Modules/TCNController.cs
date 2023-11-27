using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moon.Attributes;
using Moon.Core.Models;
using Moon.Core.Models._Imaginary;
using Moon.Core.Models.Edgerunners;
using Moon.Core.Standard;
using Moon.Core.Utilities;
using Newtonsoft.Json;

namespace Moon.Controllers.Application.NightCity.Modules
{
    [Route("api/application/night-city/modules/tcn/[action]")]
    [ApiController]
    public class TCNController : ControllerBase
    {
        private readonly ILogger<TCNController> log;
        public TCNController(ILogger<TCNController> _log)
        {
            log = _log;
        }


        #region 接收TCN信息并建单
        [Authorize]
        [PasswordCheck]
        [AuthorizeCheck(Authorization.AuthorizationEnum.Application_NightCity_Modules_TCN_TransformTCNOrder)]
        [HttpPost]
        public ControllersResult TransformTCNOrder([FromBody] OnCall_TransformTCNOrder_Parameter parameter)
        {
            ControllersResult result = new();
            try
            {
                string id = Guid.NewGuid().ToString();
                TcnTest tcnTest = new()
                {
                    Id = id,
                    CRNumber = parameter.CRNumber,
                    Title = parameter.Title,
                    Product = parameter.Product,
                    CommentMessage = parameter.CommentMessage,
                    WHPFactoryUser = parameter.WHPFactoryUser,
                    StartTime = parameter.StartTime,
                    Creator = parameter.Creator
                };
                if (parameter.Attachments != null)
                {
                    foreach (var attachment in parameter.Attachments)
                    {
                        attachment.Base64Str = string.Empty;
                    }
                    tcnTest.Attachments = JsonConvert.SerializeObject(parameter.Attachments);
                }
                Database.Edgerunners.Insertable(tcnTest).ExecuteCommand();
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

        #region TransformTCNOrder
        public class OnCall_TransformTCNOrder_Parameter
        {
            public string CRNumber { get; set; }
            public string Title { get; set; }
            public string Product { get; set; }
            public string CommentMessage { get; set; }
            public string WHPFactoryUser { get; set; }
            public DateTime StartTime { get; set; }
            public string Creator { get; set; }
            public List<_Attachment>? Attachments { get; set; }
        }
        #endregion


    }
}
