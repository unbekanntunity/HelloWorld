using API.Domain.Database.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace API.Domain.Database
{
    public class User : IdentityUser, ITagable
    {
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public string ImageUrl { get; set; }

        public List<Article> Articles { get; set; } = new();
        public List<Comment> Comments { get; set; } = new();
        public List<Post> Posts { get; set; } = new();
        public List<Project> Projects { get; set; } = new();
        public List<Message> Messages { get; set; } = new();
        public List<Reply> Replies { get; set; } = new();

        public List<Tag> Tags { get; set; } = new();
        public List<Discussion> Discussions { get; set; } = new();

        public List<Project> ProjectsJoined { get; set; } = new();

        public List<Comment> CommentsLiked { get; set; } = new();
        public List<Article> ArticlesLiked { get; set; } = new();
        public List<Project> ProjectsLiked { get; set; } = new();
        public List<Post> PostsLiked { get; set; } = new();
        public List<Reply> RepliesLiked { get; set; } = new();
    }
}
