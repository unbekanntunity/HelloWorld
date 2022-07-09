namespace HelloWorldAPI.Contracts.V1.Responses
{
    public class PartialPostResponse
    {
        public Guid Id { get; set; }

        public string Title { get; set; }
        public string Content { get; set; }

        public List<string> ImageUrls { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public string CreatorId { get; set; }

        public int UserLiked { get; set; }
        public int Comments { get; set; }

        public List<TagResponse> Tags { get; set; }
    }
}