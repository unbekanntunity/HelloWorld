namespace HelloWorldAPI.Contracts.V1.Requests
{
    public class CreatePostRequest
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public List<string> TagNames { get; set; }
        public List<IFormFile> RawImages { get; set; }
    }
}
