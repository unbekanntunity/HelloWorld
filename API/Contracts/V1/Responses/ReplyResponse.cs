namespace API.Contracts.V1.Responses
{
    public class ReplyResponse
    {
        public Guid Id { get; set; }
        public string Content { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public string CreatorId { get; set; }

        public Guid? RepliedOnId { get; set; }
        public List<ReplyResponse> Replies { get; set; }

        public List<string> UserLikedIds { get; set; }
    }
}
