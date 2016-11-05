using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace OID.Web.Authenticate
{
    public class HasSessionHandler : AuthorizationHandler<SessionRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, SessionRequirement requirement)
        {
            if (context.User.HasClaim(c => c.Type == "SessionId"))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
