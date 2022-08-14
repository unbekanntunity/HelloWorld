using API.Domain;
using API.Domain.Database;
using API.Domain.Filters;

namespace API.Services
{
    public interface IPostService
    {
        Task<Result<Post>> CreateAsync(Post post, IEnumerable<string> newTags, IEnumerable<IFormFile> images);
        Task<Result<Post>> DeleteAsync(Post post);
        Task<Result<Post>> UpdateAsync(Post post, IEnumerable<string> newTags);
        Task<Post?> GetByIdAsync(Guid id);
        Task<List<Post>> GetAllAsync(GetAllPostsFilters filter = null, PaginationFilter pagination = null);
    }
}
