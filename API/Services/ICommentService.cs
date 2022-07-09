using API.Domain;
using API.Domain.Database;
using API.Domain.Filters;

namespace API.Services
{
    public interface ICommentService
    {
        Task<Result<Comment>> CreateInPostAsync(Post Post, Comment comment);
        Task<Result<Comment>> DeleteAsync(Comment comment);
        Task<Result<Comment>> UpdateAsync(Comment comment);
        Task<Comment?> GetByIdAsync(Guid id);
        Task<List<Comment>> GetAllAsync(GetAllCommentsFilter filter = null, PaginationFilter pagination = null);
    }
}
