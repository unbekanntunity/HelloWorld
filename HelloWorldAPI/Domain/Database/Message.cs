using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HelloWorldAPI.Domain.Database
{
    public class Message : IRateable
    {
        [Key]
        public Guid Id { get; set; }
        public string Content { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public string CreatorId { get; set; }
        public User Creator { get; set; }
        public List<User> UserLiked { get; set; }
    }
}
