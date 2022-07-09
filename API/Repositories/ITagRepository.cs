using API.Domain.Database;

namespace API.Repositories
{
    public interface ITagRepository
    {
        Task<Tag?> GetByNameAsync(string name);
        Task<List<Tag>> GetAllAsync();
        Task<List<string>> GetAllNamesNoTrackingAsync();
    }
}
