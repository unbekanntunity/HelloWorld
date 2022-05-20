using HelloWorldAPI.Domain;
using HelloWorldAPI.Domain.Database;
using HelloWorldAPI.Domain.Filters;

namespace HelloWorldAPI.Services
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
