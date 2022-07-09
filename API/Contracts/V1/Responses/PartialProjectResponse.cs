namespace API.Contracts.V1.Responses
{
    public class PartialProjectResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public List<string> ImageUrls { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public string CreatorId { get; set; }
        public List<MinimalTagResponse> Tags { get; set; }
    }
}