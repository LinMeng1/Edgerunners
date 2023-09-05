using Microsoft.AspNetCore.Mvc.Filters;
using Moon.Core.Models.Edgerunners;
using Moon.Core.Models;
using System.Security.Claims;

namespace Moon.Attributes
{
    public class PasswordCheckAttribute : Attribute, IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {

        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            ClaimsPrincipal principal = context.HttpContext.User;
            string employeeID = principal.Claims.FirstOrDefault(it => it.Type == "EmployeeID").Value;
            string password = principal.Claims.FirstOrDefault(it => it.Type == "Password").Value;
            Users user = Database.Edgerunners.Queryable<Users>().First(it => it.EmployeeId == employeeID);
            if (user == null)
                context.Result = new Microsoft.AspNetCore.Mvc.UnauthorizedResult();
            else if (user.Password != password)
                context.Result = new Microsoft.AspNetCore.Mvc.UnauthorizedResult();
        }
    }
}
