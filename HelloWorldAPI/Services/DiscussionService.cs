using HelloWorldAPI.Contracts;
using HelloWorldAPI.Domain;
using HelloWorldAPI.Domain.Database;
using HelloWorldAPI.Domain.Filters;
using HelloWorldAPI.Extensions;
using HelloWorldAPI.Repositories;

namespace HelloWorldAPI.Services
{
    public class DiscussionService : IDiscussionService
    {
        private readonly IDiscussionRepository _discussionRepository;
        private readonly ITagService _tagService;
        private readonly INonQueryRepository<Discussion> _nonQueryRepository;

        public DiscussionService(IDiscussionRepository discussionRepository, ITagService tagService, INonQueryRepository<Discussion> nonQueryRepository)
        {
            _discussionRepository = discussionRepository;
            _tagService = tagService;
            _nonQueryRepository = nonQueryRepository;
        }

        public async Task<Result<Discussion>> CreateAsync(Discussion discussion, IEnumerable<string> newTags)
        {
            discussion.CreatedAt = DateTime.UtcNow;
            discussion.UpdatedAt = DateTime.UtcNow;
            var tagResult = await _tagService.CreateManyTagsForAsync(discussion, newTags.Distinct());
            if (tagResult.Success)
            {
                return new Result<Discussion>
                {
                    Success = true,
                    Data = discussion
                };
            }

            var result = await _nonQueryRepository.CreateAsync(discussion);
            return new Result<Discussion>
            {
                Success = result,
                Data = result ? discussion : null,
                Errors = result ? Array.Empty<string>() : new string[] { StaticErrorMessages<Discussion>.CreateOperationFailed },
            };
        }

        public async Task<Result<Discussion>> DeleteAsync(Guid discussionId)
        {
            var discussion = await _discussionRepository.GetByIdAsync(discussionId);
            if (discussion == null)
            {
                return new Result<Discussion>
                {
                    Errors = new string[] { StaticErrorMessages<Discussion>.NotFound }
                };
            }

            var result = await _nonQueryRepository.DeleteAsync(discussion);
            return new Result<Discussion>
            {
                Success = result,
                Errors = result ? Array.Empty<string>() : new string[] { StaticErrorMessages<Discussion>.DeleteOperationFailed }
            };
        }

        public async Task<Result<Discussion>> UpdateAsync(Discussion discussion, IEnumerable<string> newTags)
        {
            discussion.UpdatedAt = DateTime.UtcNow;

            await _tagService.UpdateTagsAsync(discussion, newTags.Distinct());

            var result = await _nonQueryRepository.UpdateAsync(discussion);
            discussion.Tags.ForEach(x => x.Discussions.Clear());

            return new Result<Discussion>
            {
                Success = true,
                Data = true ? discussion : null,
                Errors = true ? Array.Empty<string>() : new string[] { StaticErrorMessages<Discussion>.UpdateOperationFailed },
            };
        }

        public async Task<List<Discussion>> GetAllAsync(GetAllDiscussionsFilter filter = null, PaginationFilter pagination = null)
        {
            var queryable = (await _discussionRepository.GetAllAsync()).AsQueryable();
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

        public async Task<Discussion?> GetByIdAsync(Guid discussionId) => await _discussionRepository.GetByIdAsync(discussionId);

        private static IQueryable<Discussion> AddFiltersOnQuery(GetAllDiscussionsFilter filter, IQueryable<Discussion> queryable)
        {
            if (!string.IsNullOrEmpty(filter.CreatorId))
            {
                queryable = queryable.Where(x => x.CreatorId == filter.CreatorId);
            }
            if (!string.IsNullOrEmpty(filter.CreatorName))
            {
                queryable = queryable.Where(x => x.Creator.UserName == filter.CreatorName);
            }
            if (filter.ArticleId != Guid.Empty)
            {
                queryable = queryable.Where(x => x.Articles.Select(y => y.Id).Contains(filter.ArticleId));
            }
            if (!string.IsNullOrEmpty(filter.Title))
            {
                queryable = queryable.Where(x => x.Title.StartsWith(filter.Title));
            }
            if (filter.TagNames.Count != 0)
            {
                queryable = queryable.Where(x => filter.TagNames.All(y => x.Tags.Select(z => z.Name).Contains(y)));
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
