using System.Collections.Generic;

namespace NightCity.Core.Models
{
    public class Account_GetRolesAndAuthorizations_Result
    {
        public string Role { get; set; }
        public List<string> Authorizations { get; set; }
    }
}
