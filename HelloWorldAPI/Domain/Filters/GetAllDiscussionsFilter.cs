namespace HelloWorldAPI.Domain.Filters
{
    public class GetAllDiscussionsFilter
    {
        public string? CreatorId { get; set; }
        public string? CreatorName { get; set; }

        public string? Title { get; set; }
        public List<string> TagNames { get; set; } = new List<string>();

        public string? UserId { get; set; }
        public string? UserName { get; set; }

        public Guid ArticleId { get; set; }
        public string? ArticleContent { get; set; }

        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
