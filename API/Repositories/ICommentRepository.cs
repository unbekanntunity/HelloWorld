using API.Domain.Database;

namespace API.Repositories
{
    public interface ICommentRepository
    {
        Task<Comment?> GetByIdAsync(Guid id);

        Task<List<Comment>> GetAllAsync();
    }
}
