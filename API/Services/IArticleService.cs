using API.Domain;
using API.Domain.Database;
using API.Domain.Filters;

namespace API.Services
{
    public interface IArticleService
    {
        Task<Result<Article>> CreateInDiscussionAsync(Discussion Discussion, Article article);
        Task<Result<Article>> DeleteAsync(Article article);
        Task<Result<Article>> UpdateAsync(Article article);
        Task<Article?> GetByIdAsync(Guid articleId);
        Task<List<Article>> GetAllAsync(GetAllArticlesFilter filter = null, PaginationFilter pagination = null);
    }
}
