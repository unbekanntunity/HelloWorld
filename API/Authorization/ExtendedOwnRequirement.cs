using Microsoft.AspNetCore.Authorization;

namespace API.Authorization
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
