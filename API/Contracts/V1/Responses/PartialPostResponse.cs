namespace API.Contracts.V1.Responses
{
    public class PartialPostResponse
    {
        public Guid Id { get; set; }

        public string Content { get; set; }

        public List<string> ImageUrls { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public string CreatorId { get; set; }

        public List<string> UsersLikedIds { get; set; }
        public int Comments { get; set; }

        public List<string> Tags { get; set; }
    }
}