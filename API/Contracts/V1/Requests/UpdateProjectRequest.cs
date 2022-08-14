namespace API.Contracts.V1.Requests
{
    public class UpdateProjectRequest
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public List<string> TagNames { get; set; } = new();
        public List<string> MemberIds { get; set; } = new();
        public List<IFormFile> RawImages { get; set; } = new();
    }
}
