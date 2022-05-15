using HelloWorldAPI.Domain.Database;

namespace HelloWorldAPI.Repositories
{
    public interface IArticleRepository
    {
        Task<List<Article>> GetAllAsync();
        Task<Article?> GetByIdAsync(Guid articleId);
    }
}
