using API.Domain;
using API.Domain.Database;
using API.Domain.Filters;

namespace API.Services
{
    public interface IDiscussionService
    {
        Task<Result<Discussion>> CreateAsync(Discussion discussion, IEnumerable<string> newTags);
        Task<Result<Discussion>> DeleteAsync(Guid discussionId);
        Task<Result<Discussion>> UpdateAsync(Discussion discussion, IEnumerable<string> newTags);
        Task<Discussion?> GetByIdAsync(Guid discussionId);
        Task<List<Discussion>> GetAllAsync(GetAllDiscussionsFilter filter = null, PaginationFilter pagination = null);
    }
}
