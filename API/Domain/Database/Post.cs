using API.Domain.Database.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace API.Domain.Database
{
    public class Post : IRateable, ITagable
    {
        [Key]
        public Guid Id { get; set; }
        public string Content { get; set; }

        public List<ImagePath> ImagePaths { get; set; } = new();

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public string CreatorId { get; set; }
        public User Creator { get; set; }

        public List<Tag> Tags { get; set; } = new();
        public List<User> UsersLiked { get; set; } = new();
        public List<Comment> Comments { get; set; } = new();
    }
}
