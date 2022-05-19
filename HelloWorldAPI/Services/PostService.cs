using HelloWorldAPI.Contracts;
using HelloWorldAPI.Domain;
using HelloWorldAPI.Domain.Database;
using HelloWorldAPI.Domain.Filters;
using HelloWorldAPI.Extensions;
using HelloWorldAPI.Repositories;

namespace HelloWorldAPI.Services
{
    public class PostService : IPostService
    {
        private readonly IPostRepository _postRepository;
        private readonly ITagService _tagService;
        private readonly INonQueryRepository<Post> _nonQueryRepository;

        public PostService(IPostRepository postRepository, INonQueryRepository<Post> nonQueryRepository, ITagService tagService)
        {
            _postRepository = postRepository;
            _nonQueryRepository = nonQueryRepository;
            _tagService = tagService;
        }

        public async Task<Result<Post>> CreateAsync(Post post, IEnumerable<string> newTags)
        {
            post.CreatedAt = DateTime.UtcNow;
            post.UpdatedAt = DateTime.UtcNow;

            var tagResult = await _tagService.CreateManyTagsForAsync(post, newTags);
            if (tagResult.Success)
            {
                return new Result<Post>
                {
                    Success = true,
                    Data = post
                };
            }

            var result = await _nonQueryRepository.CreateAsync(post);
            return new Result<Post>
            {
                Success = result,
                Data = result ? post : null,
                Errors = result ? Array.Empty<string>() : new string[] { StaticErrorMessages<Post>.CreateOperationFailed },
            };
        }

        public async Task<Result<Post>> DeleteAsync(Post post)
        {
            var deleted = await _nonQueryRepository.DeleteAsync(post);
            return new Result<Post>
            {
                Success = deleted,
                Errors = deleted ? Array.Empty<string>() : new string[] { StaticErrorMessages<Post>.DeleteOperationFailed }
            };
        }

        public async Task<Result<Post>> UpdateAsync(Post post, IEnumerable<string> newTags)
        {
            post.UpdatedAt = DateTime.UtcNow;

            var tagResult = await _tagService.UpdateTagsAsync(post, newTags);
            if(!tagResult.Success)
            {
                return new Result<Post>
                {
                    Errors = tagResult.Errors
                };
            }

            var success = await _nonQueryRepository.UpdateAsync(post);
            return new Result<Post>
            {
                Success = success,
                Data = success ? post : null,
                Errors = success ? Array.Empty<string>() : new string[] { StaticErrorMessages<Post>.UpdateOperationFailed }
            };
        }

        public async Task<List<Post>> GetAllAsync(GetAllPostsFilters filter = null, PaginationFilter pagination = null)
        {
            var queryable = (await _postRepository.GetAllAsync()).AsQueryable();
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

        public async Task<Post?> GetByIdAsync(Guid id) => await _postRepository.GetByIdAsync(id);

        private static IQueryable<Post> AddFiltersOnQuery(GetAllPostsFilters filter, IQueryable<Post> queryable)
        {
            if (!string.IsNullOrEmpty(filter.CreatorId))
            {
                queryable = queryable.Where(x => x.Creator.Id == filter.CreatorId);
            }
            if (!string.IsNullOrEmpty(filter.CreatorName))
            {
                queryable = queryable.Where(x => x.Creator.UserName == filter.CreatorName);
            }
            if (!string.IsNullOrEmpty(filter.Title))
            {
                queryable = queryable.Where(x => x.Title.StartsWith(filter.Title));
            }
            if (!string.IsNullOrEmpty(filter.UserLikedId))
            {
                queryable = queryable.Where(x => x.UserLiked.Select(y => y.Id).Contains(filter.UserLikedId));
            }
            if (!string.IsNullOrEmpty(filter.UserLikedName))
            {
                queryable = queryable.Where(x => x.UserLiked.Select(y => y.UserName).Contains(filter.UserLikedName));
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
