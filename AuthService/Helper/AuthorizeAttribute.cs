using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Globalization;

namespace AuthService.Helper
{
    public class AuthorizeAttribute : TypeFilterAttribute
    {
        public AuthorizeAttribute(params string[] claim) : base(typeof(AuthorizeFilter))
        {
            Arguments = new object[] { claim };
        }
    }

    public class AuthorizeFilter : IAuthorizationFilter
    {
        readonly string[] _claim;

        public AuthorizeFilter(params string[] claim)
        {
            _claim = claim;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var isAuthenticated = context?.HttpContext.User.Identity.IsAuthenticated ?? false;

            if (isAuthenticated)
            {
                //bool flagClaim = false;
                bool flagClaim = true;
                var userId = context.HttpContext.User.Claims.FirstOrDefault(t => t.Type == new ClaimsIdentityOptions().UserIdClaimType).Value;
                var currentUserwiseToken = AuthHelper.UserWiseTokens.GetValueOrDefault(userId.ToString(CultureInfo.CurrentCulture));
                var userWiseToken = context.HttpContext.Request.Headers["UserWiseToken"];
                //foreach (var item in _claim)
                //{
                //    if (context.HttpContext.User.HasClaim(ClaimTypes.Role, item))
                //    {
                //        flagClaim = true;
                //        break;
                //    }
                //}
                //if (currentUserwiseToken != userWiseToken)
                //{
                //    flagClaim = false;
                //    context.Result = new JsonResult(new { result = new { message = "Unauthorized access", StatusCode = StatusCodes.Status401Unauthorized }, message = "Unauthorized access", StatusCode = StatusCodes.Status401Unauthorized }) { StatusCode = StatusCodes.Status401Unauthorized };
                //    return;
                //}

                //if (!flagClaim)
                //{
                //    context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                //    context.Result = new JsonResult(new { result = new { message = "No Permission", StatusCode = StatusCodes.Status403Forbidden }, message = "No Permission", StatusCode = StatusCodes.Status403Forbidden }) { StatusCode = StatusCodes.Status403Forbidden };
                //}
            }
            else
            {
                context.Result = new JsonResult(new { result = new { message = "Unauthorized access", StatusCode = StatusCodes.Status401Unauthorized }, message = "Unauthorized access", StatusCode = StatusCodes.Status401Unauthorized }) { StatusCode = StatusCodes.Status401Unauthorized };
            }

            return;
        }
    }
}
