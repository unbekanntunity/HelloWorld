namespace API.Domain.Filters
{
    public class GetAllProjectsFilter
    {
        public string? Title { get; set; }
        public string? CreatorId { get; set; }
        public string? CreatorName { get; set; }
        public List<string> Tags { get; set; } = new List<string>();
        public string? UserLikedId { get; set; }
        public string? UserLikedName { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
