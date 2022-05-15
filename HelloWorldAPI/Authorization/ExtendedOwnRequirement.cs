using Microsoft.AspNetCore.Authorization;

namespace HelloWorldAPI.Authorization
{
    public class ExtendedOwnRequirement : IAuthorizationRequirement
    {
        public List<string> Roles { get; set; }

        public ExtendedOwnRequirement(List<string> roles)
        {
            Roles = roles;
        }
    }
}
