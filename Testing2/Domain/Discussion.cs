namespace Testing2.Domain
{
    public class Discussion
    {
        public Guid Id { get; set; }
        public List<Tag> Tags { get; set; }
        public string Content { get; set; }
    }
}
