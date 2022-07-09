using API.Domain.Database;

namespace API.Repositories
{
    public interface IArticleRepository
    {
        Task<List<Article>> GetAllAsync();
        Task<Article?> GetByIdAsync(Guid articleId);
    }
}
