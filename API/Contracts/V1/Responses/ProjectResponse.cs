namespace API.Contracts.V1.Responses
{
    public class ProjectResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public string CreatorId { get; set; }

        public List<LinkResponse> Links { get; set; }
        public List<string> Tags { get; set; }
        public List<string> UsersLikedIds { get; set; }
        public List<string> MemberIds { get; set; }
        public List<string> ImageUrls { get; set; }
    }
}
