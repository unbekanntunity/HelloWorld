namespace API.Domain.Database
{
    public class Link
    {
        public Guid Id { get; set; }
        public Guid ProjectId { get; set; }
        public Project Project { get; set; }

        public string Name { get; set; }
        public string Value { get; set; }
    }
}
