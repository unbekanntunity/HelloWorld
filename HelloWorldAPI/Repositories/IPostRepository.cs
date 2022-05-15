using HelloWorldAPI.Domain.Database;

namespace HelloWorldAPI.Repositories
{
    public interface IPostRepository
    {
        Task<Post?> GetByIdAsync(Guid id);
        Task<List<Post>> GetAllAsync();
    }
}
