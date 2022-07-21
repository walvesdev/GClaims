using Hangfire.Annotations;
using Hangfire.Dashboard;

namespace GClaims.HangFire.Core.Filters
{
    public class CustomAuthorizationFilter : IDashboardAuthorizationFilter
    {
        string loginUrl;
        string cookieName;

        public CustomAuthorizationFilter(string loginUrl, string cookieName)
        {
            this.loginUrl = loginUrl;
            this.cookieName = cookieName;
        }

        public bool Authorize([NotNull] DashboardContext context)
        {
#if DEBUG
            return true;
#else
            var httpContext = context.GetHttpContext();

            if (httpContext.Request.Cookies.ContainsKey(cookieName))
            {
                var token = httpContext.Request.Cookies[cookieName];
                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadJwtToken(token);

                if (jsonToken.Claims.Any(p => p.Type == JwtClaimTypes.Role && p.Value == RoleDomain.ADMINISTRADOR) && jsonToken.ValidTo >= DateTime.Now)
                {
                    return true;
                }
            }

            httpContext.Response.Redirect(loginUrl);
            return true;
#endif
        }
    }
}
