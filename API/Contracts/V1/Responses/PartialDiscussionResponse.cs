namespace API.Contracts.V1.Responses
{
    public class PartialDiscussionResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string StartMessage { get; set; }
        public string CreatorId { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public List<MinimalTagResponse> Tags { get; set; }

        public MinimalArticleResponse LastArticle { get; set; }
    }
}