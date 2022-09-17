namespace API.Contracts.V1.Requests
{
    public class CreateReportRequest
    {
        public string ContentType { get; set; }
        public Guid ContentId { get; set; }
        public string Description { get; set; }
    }
}
