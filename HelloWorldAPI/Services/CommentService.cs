﻿using HelloWorldAPI.Contracts;
using HelloWorldAPI.Domain;
using HelloWorldAPI.Domain.Database;
using HelloWorldAPI.Domain.Filters;
using HelloWorldAPI.Extensions;
using HelloWorldAPI.Repositories;

namespace HelloWorldAPI.Services
{
    public class CommentService : ICommentService
    {
        private readonly IPostService _postService;
        private readonly ICommentRepository _commentRepository;
        private readonly INonQueryRepository<Comment> _nonQueryRepository;

        public CommentService(ICommentRepository commentRepository, INonQueryRepository<Comment> nonQueryRepository, IPostService postService)
        {
            _commentRepository = commentRepository;
            _nonQueryRepository = nonQueryRepository;
            _postService = postService;
        }

        public async Task<Result<Comment>> CreateInPostAsync(Guid postId, Comment comment)
        {
            var post = await _postService.GetByIdAsync(postId);
            if (post == null)
            {
                return new Result<Comment>
                {
                    Errors = new string[] { StaticErrorMessages<Post>.NotFound }
                };
            }

            comment.PostId = postId;
            var result = await _nonQueryRepository.CreateAsync(comment);
            if (result)
            {
                comment.CreatedAt = DateTime.UtcNow;
                comment.UpdatedAt = DateTime.UtcNow;
            }

            return new Result<Comment>
            {
                Success = result,
                Data = result ? comment : null,
                Errors = result ? Array.Empty<string>() : new string[] { StaticErrorMessages<Comment>.CreateOperationFailed }
            };
        }

        public async Task<Result<Comment>> DeleteAsync(Comment comment)
        {
            var deleted = await _nonQueryRepository.DeleteAsync(comment);
            return new Result<Comment>
            {
                Success = deleted,
                Errors = deleted ? Array.Empty<string>() 
                    : new string[] { StaticErrorMessages<Comment>.DeleteOperationFailed }
            };
        }

        public async Task<List<Comment>> GetAllAsync(GetAllCommentsFilter filter = null, PaginationFilter pagination = null)
        {
            var queryable = (await _commentRepository.GetAllAsync()).AsQueryable();
            if (pagination == null)
            {
                return await queryable.ToListAsyncSafe();
            }
            if (filter != null)
            {
                queryable = AddFiltersOnQuery(filter, queryable);
            }

            var skip = (pagination.PageNumber - 1) * pagination.PageSize;
            return await queryable.Skip(skip).Take(pagination.PageSize).ToListAsyncSafe();
        }

        public async Task<Result<Comment>> UpdateAsync(Comment comment)
        {
            comment.UpdatedAt = DateTime.UtcNow;
            var updated = await _nonQueryRepository.UpdateAsync(comment);
            return new Result<Comment>
            {
                Success = updated,
                Data = updated ? comment : null,
                Errors = updated ? Array.Empty<string>() :
                   new string[] { StaticErrorMessages<Comment>.UpdateOperationFailed }
            };
        }

        public async Task<Comment?> GetByIdAsync(Guid id) => await _commentRepository.GetByIdAsync(id);

        private static IQueryable<Comment> AddFiltersOnQuery(GetAllCommentsFilter filter, IQueryable<Comment> queryable)
        {
            if (!string.IsNullOrEmpty(filter.CreatorId))
            {
                queryable = queryable.Where(x => x.Creator.Id == filter.CreatorId);
            }
            if (!string.IsNullOrEmpty(filter.CreatorName))
            {
                queryable = queryable.Where(x => x.Creator.UserName == filter.CreatorName);
            }

            return queryable;
        }
    }
}
