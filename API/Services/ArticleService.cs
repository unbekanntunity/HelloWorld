﻿using API.Contracts;
using API.Domain;
using API.Domain.Database;
using API.Domain.Filters;
using API.Extensions;
using API.Repositories;

namespace API.Services
{
    public class ArticleService : IArticleService
    {
        private readonly IArticleRepository _articleRepository;
        private readonly INonQueryRepository<Article> _nonQueryRepository;

        public ArticleService(IArticleRepository articleRepository, INonQueryRepository<Article> nonQueryRepository)
        {
            _articleRepository = articleRepository;
            _nonQueryRepository = nonQueryRepository;
        }

        public async Task<Result<Article>> CreateInDiscussionAsync(Discussion discussion, Article article)
        {
            article.DiscussionId = discussion.Id;
            article.CreatedAt = DateTime.UtcNow;
            article.UpdatedAt = DateTime.UtcNow;
            var result = await _nonQueryRepository.UpdateAsync(article);

            return new Result<Article>
            {
                Success = result,
                Data = result ? article : null,
                Errors = result ? Array.Empty<string>() : new string[] { StaticErrorMessages<Article>.CreateOperationFailed },
            };
        }

        public async Task<List<Article>> GetAllAsync(GetAllArticlesFilter filter = null, PaginationFilter pagination = null)
        {
            var queryable = (await _articleRepository.GetAllAsync()).AsQueryable();

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

        public async Task<Article?> GetByIdAsync(Guid articleId) => await _articleRepository.GetByIdAsync(articleId);

        public async Task<Result<Article>> DeleteAsync(Article article)
        {
            var result = await _nonQueryRepository.DeleteAsync(article);
            return new Result<Article>
            {
                Success = result,
                Errors = result ? Array.Empty<string>() :
                    new string[] { StaticErrorMessages<Article>.DeleteOperationFailed }
            };
        }
        public async Task<Result<Article>> UpdateAsync(Article article)
        {
            article.UpdatedAt = DateTime.UtcNow;
            var updated = await _nonQueryRepository.UpdateAsync(article);

            return new Result<Article>
            {
                Success = updated,
                Data = updated ? article : null,
                Errors = updated ? Array.Empty<string>() :
                    new string[] { StaticErrorMessages<Article>.DeleteOperationFailed }
            };
        }

        private static IQueryable<Article> AddFiltersOnQuery(GetAllArticlesFilter filter, IQueryable<Article> queryable)
        {
            if (filter.DiscussionId != Guid.Empty)
            {
                queryable = queryable.Where(x => x.DiscussionId == filter.DiscussionId);
            }
            if (!string.IsNullOrEmpty(filter.CreatorId))
            {
                queryable = queryable.Where(x => x.Creator.Id == filter.CreatorId);
            }
            if (!string.IsNullOrEmpty(filter.CreatorName))
            {
                queryable = queryable.Where(x => x.Creator.UserName == filter.CreatorName);
            }
            if (!string.IsNullOrEmpty(filter.Content))
            {
                queryable = queryable.Where(x => x.Content.Contains(filter.Content));
            }
            if (!string.IsNullOrEmpty(filter.UserLikedId))
            {
                queryable = queryable.Where(x => x.UsersLiked.Select(x => x.Id).Contains(filter.UserLikedId));
            }
            if (!string.IsNullOrEmpty(filter.UserLikedName))
            {
                queryable = queryable.Where(x => x.UsersLiked.Select(x => x.UserName).Contains(filter.UserLikedName));
            }
            if (filter.CreatedAt != null)
            {
                queryable = queryable.Where(x => x.CreatedAt == filter.CreatedAt);
            }
            if (filter.UpdatedAt != null)
            {
                queryable = queryable.Where(x => x.UpdatedAt == filter.UpdatedAt);
            }
            return queryable;
        }
    }
}
