namespace HelloWorldAPI.Domain.Filters
{
    public class GetAllUserFilter
    {
        public string? UserName { get; set; }

        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public List<string> TagNames { get; set; } = new();
    }
}
