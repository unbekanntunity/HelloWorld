namespace HelloWorldAPI.Domain.Filters
{
    public class GetAllUserFilter
    {
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public List<string> TagNames { get; set; } = new();
    }
}
