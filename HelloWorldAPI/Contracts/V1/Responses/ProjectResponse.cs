namespace HelloWorldAPI.Contracts.V1.Responses
{
    public class ProjectResponse
    { 
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Desciption { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public string CreatorId { get; set; }

        public List<TagResponse> Tags { get; set; }
        public List<string> UserLikedIds { get; set; }
        public List<string> MemberIds { get; set; }
    }
}
