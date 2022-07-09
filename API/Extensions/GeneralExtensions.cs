using System.Security.Claims;

namespace API.Extensions
{
    public static class GeneralExtensions
    {
        public static string GetUserId(this HttpContext httpContext)
        {
            if (httpContext.User == null)
            {
                return string.Empty;
            }

            return httpContext.User.Claims.Single(x => x.Type == "id").Value;
        }

        public static bool HasRole(this HttpContext httpContext, string roleName)
        {
            if (httpContext.User == null)
            {
                return false;
            }

            return httpContext.User.Claims.Where(x => x.Type == ClaimTypes.Role).Select(x => x.Value).Contains(roleName);
        }
    }
}
