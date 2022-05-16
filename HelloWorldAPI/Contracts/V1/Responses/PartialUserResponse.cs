namespace HelloWorldAPI.Contracts.V1.Responses
{
    public class PartialUserResponse
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Description { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }


        public List<string> Roles { get; set; }
    }
}
