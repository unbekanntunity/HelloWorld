namespace API.Domain.Database
{
    public class Report
    {
        public Guid Id { get; set; }

        public string CreatorId { get; set; }

        public Guid ContentId { get; set; }

        public string ContentType { get; set; }
        public string Description { get; set; }


        public string? ModId { get; set; }
        public string Status { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
