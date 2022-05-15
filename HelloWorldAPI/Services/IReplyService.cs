using HelloWorldAPI.Domain;
using HelloWorldAPI.Domain.Database;
using HelloWorldAPI.Domain.Filters;

namespace HelloWorldAPI.Services
{
    public interface IReplyService
    {
        Task<Result<Reply>> AddReplyForReplyAsync(Reply repliedOn, Reply reply);
        Task<Result<Reply>> AddReplyForCommentAsync(Comment comment, Reply reply);
        Task<Result<Reply>> DeleteAsync(Reply reply);
        Task<Result<Reply>> UpdateAsync(Reply reply);

        Task<Reply?> GetByIdAsync(Guid id);
        Task<List<Reply>> GetAllAsync(GetAllRepliesFilter filter = null, PaginationFilter pagination = null);
    }
}
