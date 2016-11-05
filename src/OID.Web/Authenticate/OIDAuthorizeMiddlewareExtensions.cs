using System;
using Microsoft.AspNetCore.Builder;

namespace OID.Web.Authenticate
{
    public static class OIDAuthorizeMiddlewareExtensions
    {
        public static IApplicationBuilder UseOIDAuthroziation(this IApplicationBuilder app)
        {
            if (app == null)
                throw new ArgumentNullException(nameof(app));

            return app.UseMiddleware<OIDAuthorizeMiddleware>();
        }
    }
}
