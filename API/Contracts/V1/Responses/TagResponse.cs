namespace API.Contracts.V1.Responses
{
    public class TagResponse
    {
        public string Name { get; set; }

        public int DiscussionsTaged { get; set; }
        public int PostsTaged { get; set; }
        public int ProjectsTaged { get; set; }
        public int UsersTaged { get; set; }
    }
}