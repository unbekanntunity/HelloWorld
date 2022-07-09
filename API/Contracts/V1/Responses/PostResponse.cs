using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace API.Contracts.V1.Responses
{
    public class PostResponse
    {
        public Guid Id { get; set; }

        public string Title { get; set; }
        public string Content { get; set; }

        public List<string> ImageUrls { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public string CreatorId { get; set; }

        public List<MinimalTagResponse> Tags { get; set; } = new();
        public List<string> UserLikedIds { get; set; } = new();
        public List<CommentResponse> Comments { get; set; } = new();
    }
}