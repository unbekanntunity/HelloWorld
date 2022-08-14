namespace API.Contracts.V1.Responses
{
    public class SavedResponse
    {
        public List<PostResponse> SavedPosts { get; set; }
        public List<DiscussionResponse> SavedDiscussions { get; set; }
        public List<ProjectResponse> SavedProjects { get; set; }
    }
}
