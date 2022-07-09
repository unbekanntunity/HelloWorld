using API.Domain.Database;

namespace API.Repositories
{
    public interface IProjectRepository
    {
        Task<Project?> GetByIdAsync(Guid id);
        Task<List<Project>> GetAllAsync();
    }
}
