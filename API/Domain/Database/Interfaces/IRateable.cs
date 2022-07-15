using API.Domain.Database;

namespace API.Domain.Database.Interfaces
{
    public interface IRateable
    {
        public List<User> UsersLiked { get; set; }
    }
}
