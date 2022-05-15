namespace HelloWorldAPI.Contracts.V1.Requests
{
    public class UpdateLoginRequest
    {
        public string Email { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
