namespace API.Contracts.V1.Requests
{
    public class UpdateUserRequest
    {
        public string UserName { get; set; }
        public string Description { get; set; }

        public List<string> TagNames { get; set; } = new List<string>();
        public IFormFile? Image { get; set; }

    }
}
