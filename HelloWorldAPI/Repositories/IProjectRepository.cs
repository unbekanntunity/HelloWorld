using HelloWorldAPI.Domain.Database;

namespace HelloWorldAPI.Repositories
{
    public interface IProjectRepository
    {
        Task<Project?> GetByIdAsync(Guid id);
        Task<List<Project>> GetAllAsync();
    }
}
