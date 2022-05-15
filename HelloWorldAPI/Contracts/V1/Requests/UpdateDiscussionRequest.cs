namespace HelloWorldAPI.Contracts.V1.Requests
{
    public class UpdateDiscussionRequest
    {
        public List<string> Tags { get; set; } = new List<string>();
    }
}
