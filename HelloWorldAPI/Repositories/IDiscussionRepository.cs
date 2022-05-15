using HelloWorldAPI.Domain.Database;

namespace HelloWorldAPI.Repositories
{
    public interface IDiscussionRepository
    {
        Task<List<Discussion>> GetAllAsync();
        Task<Discussion?> GetByIdAsync(Guid id);
    }
}
