using HelloWorldAPI.Domain.Database;

namespace HelloWorldAPI.Repositories
{
    public interface ITagRepository
    {
        Task<Tag?> GetByNameAsync(string name);
        Task<List<Tag>> GetAllAsync();
        Task<List<string>> GetAllNamesNoTrackingAsync();
    }
}
