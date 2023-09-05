using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Moon.Core.Models.Edgerunners;
using Moon.Core.Models;
using System.Security.Claims;
using Moon.Core.Utilities;
using static Moon.Core.Utilities.Authorization;

namespace Moon.Attributes
{
    public class AuthorizeCheckAttribute : Attribute, IActionFilter
    {
        private AuthorizationEnum authorization;
        public AuthorizeCheckAttribute(Authorization.AuthorizationEnum authorization)
        {
            this.authorization = authorization;
        }
        public void OnActionExecuted(ActionExecutedContext context)
        {

        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            ClaimsPrincipal principal = context.HttpContext.User;
            string employeeID = principal.Claims.FirstOrDefault(it => it.Type == "EmployeeID").Value;
            List<int> list_roles_authorizations = Database.Edgerunners.Queryable<Roles_Authorizations>().Where(it => it.AuthorizationEnum == (int)authorization || it.AuthorizationEnum == (int)AuthorizationEnum.Default).Select(it => it.RoleId).ToList();
            Users_Roles users_roles = Database.Edgerunners.Queryable<Users_Roles>().First(it => it.UserEmployeeId == employeeID && (list_roles_authorizations.Contains(it.RoleId)));
            if (users_roles == null)
            {
                bool result = false;
                string errorMessage = $"Unauthorized requests : {authorization} , Please check your account ({employeeID})";
                context.Result = new OkObjectResult(new { result, errorMessage });
            }
        }
    }
}
