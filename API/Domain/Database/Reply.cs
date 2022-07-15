using API.Domain.Database.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace API.Domain.Database
{
    public class Reply : IRateable
    {
        [Key]
        public Guid Id { get; set; }
        public string Content { get; set; }

        public string CreatorId { get; set; }
        public User Creator { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public Guid? RepliedOnArticleId { get; set; }
        public Article? RepliedOnArticle { get; set; }

        public Guid? RepliedOnCommentId { get; set; }
        public Comment? RepliedOnComment { get; set; }

        public Guid? RepliedOnReplyId { get; set; }
        public Reply? RepliedOnReply { get; set; }

        public List<Reply> Replies { get; set; } = new();

        public List<User> UsersLiked { get; set; } = new();
    }
}
