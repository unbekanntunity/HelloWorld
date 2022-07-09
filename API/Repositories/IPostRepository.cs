using API.Domain.Database;

namespace API.Repositories
{
    public interface IPostRepository
    {
        Task<Post?> GetByIdAsync(Guid id);
        Task<List<Post>> GetAllAsync();
    }
}
