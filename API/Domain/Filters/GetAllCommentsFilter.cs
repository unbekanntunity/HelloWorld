namespace API.Domain.Filters
{
    public class GetAllCommentsFilter
    {
        public string? Content { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public string? CreatorId { get; set; }
        public string? CreatorName { get; set; }

        public Guid PostId { get; set; }

        public string? UserLikedId { get; set; }
        public string? UserLikedName { get; set; }
    }
}

