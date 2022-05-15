using HelloWorldAPI.Contracts;
using HelloWorldAPI.Domain;
using HelloWorldAPI.Domain.Database;
using HelloWorldAPI.Domain.Filters;
using HelloWorldAPI.Extensions;
using HelloWorldAPI.Repositories;

namespace HelloWorldAPI.Services
{
    public class ArticleService : IArticleService
    {
        private readonly IArticleRepository _articleRepository;
        private readonly IDiscussionService _discussionService;
        private readonly INonQueryRepository<Article> _nonQueryRepository;

        public ArticleService(IArticleRepository articleRepository, IDiscussionService discussionService, INonQueryRepository<Article> nonQueryRepository)
        {
            _articleRepository = articleRepository;
            _discussionService = discussionService;
            _nonQueryRepository = nonQueryRepository;
        }

        public async Task<Result<Article>> CreateInDiscussionAsync(Guid discussionId, Article article)
        {
            var discussion = await _discussionService.GetByIdAsync(discussionId);
            if (discussion == null)
            {
                return new Result<Article>
                {
                    Errors = new string[] { StaticErrorMessages<Discussion>.NotFound }
                };
            }

            article.DiscussionId = discussionId;
            article.CreatedAt = DateTime.UtcNow;
            article.UpdatedAt = DateTime.UtcNow;
            var result = await _nonQueryRepository.CreateAsync(article);

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
            if(!string.IsNullOrEmpty(filter.CreatorId))
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
                queryable = queryable.Where(x => x.UserLiked.Select(x => x.Id).Contains(filter.UserLikedId));
            }
            if (!string.IsNullOrEmpty(filter.UserLikedName))
            {
                queryable = queryable.Where(x => x.UserLiked.Select(x => x.UserName).Contains(filter.UserLikedName));
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
