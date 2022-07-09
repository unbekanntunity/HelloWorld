using API.Domain.Database;

namespace API.Domain.Database.Interfaces
{
    public interface IRateable
    {
        public List<User> UserLiked { get; set; }
    }
}
