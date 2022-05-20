namespace HelloWorldAPI.Contracts.V1.Requests
{
    public class CreateProjectRequest
    {
        public string Title { get; set; }
        public string Desciption { get; set; }
        
        public List<string> TagNames { get; set; }
        public List<string> MembersIds { get; set; }
    }
}
