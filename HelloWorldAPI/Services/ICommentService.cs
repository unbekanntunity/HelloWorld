using HelloWorldAPI.Domain;
using HelloWorldAPI.Domain.Database;
using HelloWorldAPI.Domain.Filters;

namespace HelloWorldAPI.Services
{
    public interface ICommentService
    {
        Task<Result<Comment>> CreateInPostAsync(Guid postId, Comment comment);
        Task<Result<Comment>> DeleteAsync(Comment comment);
        Task<Result<Comment>> UpdateAsync(Comment comment);
        Task<Comment?> GetByIdAsync(Guid id);
        Task<List<Comment>> GetAllAsync(GetAllCommentsFilter filter = null, PaginationFilter pagination = null);
    }
}
