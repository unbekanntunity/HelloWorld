using API.Domain.Database.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace API.Domain.Database
{
    public class Article : IRateable
    {
        [Key]
        public Guid Id { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public string CreatorId { get; set; }
        public User Creator { get; set; }

        public Guid DiscussionId { get; set; }
        public Discussion Discussion { get; set; }

        public List<User> UserLiked { get; set; } = new();
        public List<Reply> Replies { get; set; } = new();
    }
}
