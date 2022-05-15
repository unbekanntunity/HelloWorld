namespace HelloWorldAPI.Domain.Filters
{
    public class GetAllPostsFilters
    {
        public string? Title { get; set; }

        public string? CreatorId { get; set; }
        public string? CreatorName { get; set; }

        public List<string> TagNames { get; set; } = new();

        public string? UserLikedId { get; set; }
        public string? UserLikedName { get; set; }
        public List<string> CommentsContent { get; set; } = new();

        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
