using HelloWorldAPI.Contracts;
using HelloWorldAPI.Domain;
using HelloWorldAPI.Domain.Database;
using HelloWorldAPI.Domain.Filters;
using HelloWorldAPI.Extensions;
using HelloWorldAPI.Repositories;

namespace HelloWorldAPI.Services
{
    public class ReplyService : IReplyService
    {
        private readonly IReplyRepository _replyRepository;
        private readonly INonQueryRepository<Article> _nonQueryArticleRepository;
        private readonly INonQueryRepository<Reply> _nonQueryReplyRepository;
        private readonly INonQueryRepository<Comment> _nonQueryCommentRepository;

        public ReplyService(IReplyRepository replyRepository, INonQueryRepository<Reply> nonQueryReplyRepository, INonQueryRepository<Comment> nonQueryCommentRepository, INonQueryRepository<Article> nonQueryArticleRepository)
        {
            _replyRepository = replyRepository;
            _nonQueryReplyRepository = nonQueryReplyRepository;
            _nonQueryCommentRepository = nonQueryCommentRepository;
            _nonQueryArticleRepository = nonQueryArticleRepository;
        }

        public async Task<Result<Reply>> AddReplyForArticleAsync(Article article, Reply reply)
        {
            reply.CreatedAt = DateTime.UtcNow;
            reply.UpdatedAt = DateTime.UtcNow;
            article.Replies.Add(reply);

            var updated = await _nonQueryArticleRepository.UpdateAsync(article);

            reply.RepliedOnArticle = article;
            reply.RepliedOnArticleId = article.Id;

            return new Result<Reply>
            {
                Success = updated,
                Data = updated ? reply : null,
                Errors = updated ? Array.Empty<string>() : new string[] { StaticErrorMessages<Reply>.CreateOperationFailed }
            };
        }

        public async Task<Result<Reply>> AddReplyForReplyAsync(Reply repliedOn, Reply reply)
        {
            reply.CreatedAt = DateTime.UtcNow;
            reply.UpdatedAt = DateTime.UtcNow;
            repliedOn.Replies.Add(reply);

            var updated = await _nonQueryReplyRepository.UpdateAsync(repliedOn);

            reply.RepliedOnReply = repliedOn;
            reply.RepliedOnReplyId = repliedOn.Id;

            return new Result<Reply>
            {
                Success = updated,
                Data = updated ? reply : null,
                Errors = updated ? Array.Empty<string>() : new string[] { StaticErrorMessages<Reply>.CreateOperationFailed }
            };
        }

        public async Task<Result<Reply>> AddReplyForCommentAsync(Comment comment, Reply reply)
        {
            reply.CreatedAt = DateTime.UtcNow;
            reply.UpdatedAt = DateTime.UtcNow;

            comment.Replies.Add(reply);

            var updated = await _nonQueryCommentRepository.UpdateAsync(comment);

            reply.RepliedOnComment = comment;
            reply.RepliedOnCommentId = comment.Id;

            return new Result<Reply>
            {
                Success = updated,
                Data = updated ? reply : null,
                Errors = updated ? Array.Empty<string>() : new string[] { StaticErrorMessages<Reply>.CreateOperationFailed }
            };
        }

        public async Task<Result<Reply>> DeleteAsync(Reply reply)
        {
            var result = await _nonQueryReplyRepository.DeleteAsync(reply);
            return new Result<Reply>
            {
                Success = result,
                Errors = result ? Array.Empty<string>() :
                    new string[] { StaticErrorMessages<Reply>.DeleteOperationFailed }
            };
        }
        
        public async Task<List<Reply>> GetAllAsync(GetAllRepliesFilter filter = null, PaginationFilter pagination = null)
        {
            var queryable = (await _replyRepository.GetAllAsync()).AsQueryable();
            if (pagination == null)
            {
                return await queryable.ToListAsyncSafe();
            }
            if (filter != null)
            {
                queryable = AddFiltersOnQuery(filter, queryable);
            }

            var skip = (pagination.PageNumber - 1) * pagination.PageSize;
            return await queryable
                .Skip(skip)
                .Take(pagination.PageSize)
                .Where(x => x.RepliedOnReply == null)
                .ToListAsyncSafe();
        }
        public async Task<Reply?> GetByIdAsync(Guid id) => await _replyRepository.GetByIdAsync(id);

        public async Task<Result<Reply>> UpdateAsync(Reply reply)
        {
            reply.UpdatedAt = DateTime.UtcNow;

            var updated = await _nonQueryReplyRepository.UpdateAsync(reply);
            return new Result<Reply>
            {
                Success = updated,
                Data = updated ? reply : null,
                Errors = updated ? Array.Empty<string>()
                    : new string[] { StaticErrorMessages<Reply>.UpdateOperationFailed }
            };
        }

        public static IQueryable<Reply> AddFiltersOnQuery(GetAllRepliesFilter filter, IQueryable<Reply> queryable)
        {
            if (!string.IsNullOrEmpty(filter.CreatorId))
            {
                queryable = queryable.Where(x => x.Creator.Id == filter.CreatorId);
            }
            if (!string.IsNullOrEmpty(filter.CreatorName))
            {
                queryable = queryable.Where(x => x.Creator.UserName == filter.CreatorName);
            }
            if (filter.RepliedOnCommentId != Guid.Empty)
            {
                queryable = queryable.Where(x => x.RepliedOnArticleId == filter.RepliedOnArticleId);
            }
            if (filter.RepliedOnCommentId != Guid.Empty)
            {
                queryable = queryable.Where(x => x.RepliedOnCommentId == filter.RepliedOnCommentId);
            }
            if (filter.RepliedOnReplyId != Guid.Empty)
            {
                queryable = queryable.Where(x => x.RepliedOnReplyId == filter.RepliedOnReplyId);
            }
            if (!string.IsNullOrEmpty(filter.Content))
            {
                queryable = queryable.Where(x => x.Content.Contains(filter.Content));
            }

            return queryable;
        }
    }
}
