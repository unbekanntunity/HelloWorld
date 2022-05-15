namespace HelloWorldAPI.Contracts.V1.Requests
{
    public class UpdateProjectRequest
    {
        public string Title { get; set; }
        public string Desciption { get; set; }
        public List<string> Tags { get; set; } = new();
        public List<string> MemberIds { get; set; } = new();
    }
}
