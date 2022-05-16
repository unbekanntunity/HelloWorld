using System.ComponentModel.DataAnnotations;

namespace HelloWorldAPI.Contracts.V1.Requests
{
    public class UserRegistrationRequest
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
