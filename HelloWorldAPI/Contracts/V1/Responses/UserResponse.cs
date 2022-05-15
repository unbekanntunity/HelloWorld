namespace HelloWorldAPI.Contracts.V1.Responses
{
    public class UserResponse
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Description { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }


        public List<string> Roles { get; set; }

        public List<PartialProjectResponse> Projects { get; set; }
        public List<PartialDiscussionResponse> Discussions { get; set; }
        public List<PartialPostResponse> Posts { get; set; }
        public List<TagResponse> Tags { get; set; }
   }
}
