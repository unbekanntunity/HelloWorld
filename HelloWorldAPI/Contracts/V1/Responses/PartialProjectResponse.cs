namespace HelloWorldAPI.Contracts.V1.Responses
{
    public class PartialProjectResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public string CreatorId { get; set; }
        public List<TagResponse> Tags { get; set; }

        public int Members { get; set; }
        public int UserLiked { get; set; }
    }
}