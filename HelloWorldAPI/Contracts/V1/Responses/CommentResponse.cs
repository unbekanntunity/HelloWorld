namespace HelloWorldAPI.Contracts.V1.Responses
{
    public class CommentResponse
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public string CreatorId { get; set; }
        public Guid PostId { get; set; }

        public List<string> UserLikedIds { get; set; } = new();

        public List<ReplyResponse> Replies { get; set; } = new();
    }
}
