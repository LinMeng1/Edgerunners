using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moon.Core.Models;
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
        }
        #endregion

        #region GetOfficeToken
        public class Authorize_GetOfficeToken_Parameter
        {
            public string Mainboard { get; set; }
        }

        #endregion
    }
}
