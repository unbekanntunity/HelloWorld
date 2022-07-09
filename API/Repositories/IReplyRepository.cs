using API.Domain.Database;

namespace API.Repositories
{
    public interface IReplyRepository
    {
        Task<Reply?> GetByIdAsync(Guid id);
        Task<List<Reply>> GetAllAsync();
    }
}
