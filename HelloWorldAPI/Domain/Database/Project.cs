using System.ComponentModel.DataAnnotations;

namespace HelloWorldAPI.Domain.Database
{
    public class Project : IRateable
    {
        [Key]
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Desciption { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public string CreatorId { get; set; }
        public User Creator { get; set; }

        public List<Tag> Tags { get; set; } = new();
        public List<User> Members { get; set; } = new();

        public List<User> UserLiked { get; set; } = new();
    }
}
