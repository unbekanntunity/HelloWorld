using API.Domain.Database.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace API.Domain.Database
{
    public class Project : IRateable, ITagable, ISavable
    {
        [Key]
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Desciption { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public string CreatorId { get; set; }
        public User Creator { get; set; }

        public List<Link> Links { get; set; } = new();
        public List<Tag> Tags { get; set; } = new();
        public List<User> Members { get; set; } = new();
        public List<User> UsersLiked { get; set; } = new();
        public List<User> SavedBy { get; set; } = new();
        public List<ImagePath> ImagePaths { get; set; } = new();
    }
}
