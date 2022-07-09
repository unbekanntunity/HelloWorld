using API.Domain;
using API.Domain.Database;
using API.Domain.Filters;

namespace API.Services
{
    public interface IReplyService
    {
        Task<Result<Reply>> AddReplyForArticleAsync(Article article, Reply reply);
        Task<Result<Reply>> AddReplyForReplyAsync(Reply repliedOn, Reply reply);
        Task<Result<Reply>> AddReplyForCommentAsync(Comment comment, Reply reply);
        Task<Result<Reply>> DeleteAsync(Reply reply);
        Task<Result<Reply>> UpdateAsync(Reply reply);

        Task<Reply?> GetByIdAsync(Guid id);
        Task<List<Reply>> GetAllAsync(GetAllRepliesFilter filter = null, PaginationFilter pagination = null);
    }
}
