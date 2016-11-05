using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace OID.Web.Authenticate
{
    public class OIDAuthorizeMiddleware
    {
        private readonly RequestDelegate _next;

        public OIDAuthorizeMiddleware(RequestDelegate next)
        {
            _next = next;

        }

        public async Task Invoke(HttpContext context)
        {
            // TODO как протащить контекст
            var userManager = new UserManager(context);
            var user = userManager.GetUser();

            if (user != null)
            {
                var claimsIdentity = new List<ClaimsIdentity>
                {
                    new ClaimsIdentity(new List<Claim>
                    {
                        new Claim("SessionId", user.SessionId)
                    })
                };

                var claims = new ClaimsPrincipal(claimsIdentity);
                await context.Authentication.SignInAsync("SessionId", claims);
            }

            await _next.Invoke(context);
        }
    }
}
