namespace HelloWorldAPI.Contracts.V1.Requests
{
    public class CreateUserRequest
    {

        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public string Description { get; set; }
        public string[] RoleNames { get; set; }
    }
}
