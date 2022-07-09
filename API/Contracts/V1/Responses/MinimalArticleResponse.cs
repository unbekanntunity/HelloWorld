namespace API.Contracts.V1.Responses
{
    public class MinimalArticleResponse
    {
        public Guid Id { get; set; }

        public string CreatorId { get; set; }
        public Guid DiscussionId { get; set; }

        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

    }
}
