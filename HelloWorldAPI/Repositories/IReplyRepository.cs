using HelloWorldAPI.Domain.Database;
using HelloWorldAPI.Domain.Filters;

namespace HelloWorldAPI.Repositories
{
    public interface IReplyRepository
    {
        Task<Reply?> GetByIdAsync(Guid id);
        Task<List<Reply>> GetAllAsync();
    }
}
