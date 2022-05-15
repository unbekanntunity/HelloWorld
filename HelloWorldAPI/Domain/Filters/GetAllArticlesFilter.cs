namespace HelloWorldAPI.Domain.Filters
{
    public class GetAllArticlesFilter
    {
        public Guid DiscussionId { get; set; }
        public string? Content { get; set; }

        public string? UserLikedId { get; set; }
        public string? UserLikedName { get; set; }

        public string? CreatorId { get; set; }
        public string? CreatorName { get; set; }

        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
