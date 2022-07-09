using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace API.Authorization
{
    public class ExtendedOwnHandler : AuthorizationHandler<ExtendedOwnRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ExtendedOwnRequirement requirement)
        {
            context.Fail();
            return Task.CompletedTask;
        }
    }
}
