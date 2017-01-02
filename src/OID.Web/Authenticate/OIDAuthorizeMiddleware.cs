using System.Collections.Generic;
using System.Linq;
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
            else if (context.User.HasClaim(c => c.Type == "SessionId"))
            {
                var claim = context.User.Claims.Single(c => c.Type == "SessionId");
                ((ClaimsIdentity) context.User.Identity).RemoveClaim(claim);
            }

            await _next.Invoke(context);
        }
    }
}
