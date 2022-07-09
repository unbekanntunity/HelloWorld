using API.Domain.Database.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace API.Domain.Database
{
    public class Tag
    {
        [Key]
        public string Name { get; set; }

        public List<Discussion> Discussions { get; set; } = new();
        public List<Post> Posts { get; set; } = new();
        public List<Project> Projects { get; set; } = new();
        public List<User> Users { get; set; } = new();
    }
}
