using API.Domain.Database;

namespace API.Repositories
{
    public interface IDiscussionRepository
    {
        Task<List<Discussion>> GetAllAsync();
        Task<Discussion?> GetByIdAsync(Guid id);
    }
}
