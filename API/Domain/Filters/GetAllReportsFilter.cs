namespace API.Domain.Filters
{
    public class GetAllReportsFilter
    {
        public string? CreatorId { get; set; }
        public string? ModId { get; set; }
        public string? Status { get; set; }

        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
