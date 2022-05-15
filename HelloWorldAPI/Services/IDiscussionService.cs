using HelloWorldAPI.Domain;
using HelloWorldAPI.Domain.Database;
using HelloWorldAPI.Domain.Filters;

namespace HelloWorldAPI.Services
{
    public interface IDiscussionService
    {
        Task<Result<Discussion>> CreateAsync(Discussion discussion, IEnumerable<string> newTags);
        Task<Result<Discussion>> UpdateAsync(Discussion discussion, IEnumerable<string> newTags);
        Task<Result<Discussion>> DeleteAsync(Guid discussionId);
        Task<Discussion?> GetByIdAsync(Guid discussionId);
        Task<List<Discussion>> GetAllAsync(GetAllDiscussionsFilter filter = null, PaginationFilter pagination = null);
    }
}
