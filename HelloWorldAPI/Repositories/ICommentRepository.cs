using HelloWorldAPI.Domain.Database;

namespace HelloWorldAPI.Repositories
{
    public interface ICommentRepository
    {
        Task<Comment?> GetByIdAsync(Guid id);

        Task<List<Comment>> GetAllAsync();
    }
}
