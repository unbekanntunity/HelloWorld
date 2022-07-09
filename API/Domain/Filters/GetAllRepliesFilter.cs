namespace API.Domain.Filters
{
    public class GetAllRepliesFilter
    {
        public string? Content { get; set; }
        public string? CreatorId { get; set; }
        public string? CreatorName { get; set; }
        public Guid RepliedOnArticleId { get; set; }
        public Guid RepliedOnReplyId { get; set; }
        public Guid RepliedOnCommentId { get; set; }
    }
}
