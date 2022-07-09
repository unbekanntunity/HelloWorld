using API.Domain.Database;

namespace API.Domain.Database.Interfaces
{
    public interface ITagable
    {
        public List<Tag> Tags { get; set; }
    }
}
