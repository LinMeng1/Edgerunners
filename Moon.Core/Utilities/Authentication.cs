using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Moon.Core.Utilities
{
    public class Authentication
    {
        #region Token
        public static string SecretKey { get; set; } = "I Really Want to Stay At Your House";
        public static string GetJWT(string employeeID, string password)
        {
            // 密钥
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            //声明
            var claims = new List<Claim>
            {
                new Claim("EmployeeID",employeeID),
                new Claim("Password",password),
                new Claim(JwtRegisteredClaimNames.Iss,"Moon.Core"),
                new Claim(JwtRegisteredClaimNames.Aud,"Edgerunners"),             
                // 令牌颁发时间
                new Claim(JwtRegisteredClaimNames.Iat, $"{new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds()}"),
                new Claim(JwtRegisteredClaimNames.Nbf,$"{new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds()}"),
                 // 过期时间 365天
                new Claim(JwtRegisteredClaimNames.Exp,$"{new DateTimeOffset(DateTime.Now.AddDays(365)).ToUnixTimeSeconds()}"),
            };
            JwtSecurityToken jwt = new(
                claims: claims,
                signingCredentials: creds
                );
            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }
        #endregion

        #region ToMD5
        public static string ToMD5(string plaintext)
        {
            MD5 md5 = MD5.Create();
            byte[] palindata = Encoding.Default.GetBytes(plaintext);
            byte[] encryptdata = md5.ComputeHash(palindata);
            return Convert.ToBase64String(encryptdata);
        }
        #endregion
    }
}
