using HelloWorldAPI.Domain;
using HelloWorldAPI.Domain.Database;
using HelloWorldAPI.Domain.Filters;

namespace HelloWorldAPI.Services
{
    public interface IPostService
    {
        Task<Result<Post>> CreateAsync(Post post, IEnumerable<string> newTags);
        Task<Result<Post>> DeleteAsync(Post post);
        Task<Result<Post>> UpdateAsync(Post post, IEnumerable<string> newTags);
        Task<Post?> GetByIdAsync(Guid id);
        Task<List<Post>> GetAllAsync(GetAllPostsFilters filter = null, PaginationFilter pagination = null);

    }
}
