using API.Domain.Database;

namespace API.Contracts.V1.Responses
{
    public class UserResponse
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Description { get; set; }

        public string ImageUrl { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public List<string> Roles { get; set; }
        public List<string> Tags { get; set; }

        public List<string> FollowerIds { get; set; }
        public List<string> FollowingIds { get; set; }

        public List<PostResponse> Posts { get; set; }
        public List<DiscussionResponse> Discussions { get; set; }
        public List<ProjectResponse> Projects { get; set; }

        public List<PostResponse> SavedPosts { get; set; }
        public List<DiscussionResponse> SavedDiscussions { get; set; }
        public List<ProjectResponse> SavedProjects { get; set; }
    }
}
