using Microsoft.AspNetCore.Mvc;
using Moon.Core.Models;
using Moon.Core.Models._Imaginary;
using Moon.Core.Models.Edgerunners;
using Moon.Core.Standard;
using Moon.Core.Utilities;

namespace Moon.Controllers.Basic
{
    [Route("api/basic/authorize/[action]")]
    [ApiController]
    public class AuthorizeController : ControllerBase
    {
        private readonly ILogger<AuthorizeController> log;
        public AuthorizeController(ILogger<AuthorizeController> _log)
        {
            log = _log;
        }

        #region 获取登录Token
        [HttpPost]
        public ControllersResult GetToken([FromBody] Authorize_GetToken_Parameter parameter)
        {
            ControllersResult result = new();
            string token = string.Empty;
            try
            {
                string username = parameter.Username;
                string password = parameter.Password;
                Users user = Database.Edgerunners.Queryable<Users>().First(it => it.EmployeeId == username || it.ItCode == username);
                if (user == null)
                    throw new Exception($"Invalid username ({username}) , please login with EmployeeID or ItCode as username");
                else if (user.Password != Authentication.ToMD5(password))
                    throw new Exception("Invalid password , please recheck");
                token = Authentication.GetJWT(user.EmployeeId, user.Password);
                if (parameter.Client != null)
                {
                    var lastLogin = Database.Edgerunners.Queryable<NightCityLoginHistories>().OrderBy(it => it.Time, SqlSugar.OrderByType.Desc).First();
                    NightCityLoginHistories nightCityLoginHistories = new()
                    {
                        User = user.EmployeeId,
                        Client = parameter.Client
                    };
                    Database.Edgerunners.Insertable(nightCityLoginHistories).IgnoreColumns("Id").IgnoreColumns("Time").ExecuteCommand();
                    if (lastLogin != null)
                    {
                        try
                        {
                            _ = Mqtt.Publish(lastLogin.Client, new _MqttMessage()
                            {
                                IsMastermind = true,
                                Address = "Moon",
                                Sender = "Lucy",
                                Content = "system clean te authorization info"
                            });
                        }
                        catch { }
                    }
                }
                result.Content = new { token, user.EmployeeId, user.Name };
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

        #region GetToken
        public class Authorize_GetToken_Parameter
        {
            public string Username { get; set; }
            public string Password { get; set; }
            public string? Client { get; set; }
        }
        #endregion
    }
}
