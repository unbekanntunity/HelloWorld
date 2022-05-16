namespace HelloWorldAPI.Contracts.V1.Responses
{
    public class ArticleResponse
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }

        public string CreatorId { get; set; }

        public Guid DiscussionId { get; set; }

        public List<string> UserLikedIds { get; set; }
        public List<ReplyResponse> Replies { get; set; }
    }
}
