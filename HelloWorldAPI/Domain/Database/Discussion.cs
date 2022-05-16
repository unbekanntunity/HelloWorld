using HelloWorldAPI.Domain.Database.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace HelloWorldAPI.Domain.Database
{
    public class Discussion : ITagable
    {
        [Key]
        public Guid Id { get; set; }

        public string Title { get; set; }
        public string StartMessage { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public string CreatorId { get; set; }
        public User Creator { get; set; }

        public List<Tag> Tags { get; set; } = new();
        public List<Article> Articles { get; set; } = new();
    }
}
