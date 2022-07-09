using API.Contracts;
using API.Domain;
using API.Domain.Database;
using API.Domain.Filters;
using API.Extensions;
using API.Repositories;

namespace API.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IIdentityService _identityService;
        private readonly IProjectRepository _projectRepository;
        private readonly ITagService _tagService;
        private readonly INonQueryRepository<Project> _nonQueryRepository;
        private readonly INonQueryRepository<ImagePath> _nonQuery;
        private readonly IFileManager _fileManager;

        public ProjectService(IProjectRepository projectRepository, IIdentityService identityService, ITagService tagService, INonQueryRepository<Project> nonQueryRepository, IFileManager fileManager, INonQueryRepository<ImagePath> nonQueryRepository1)
        {
            _projectRepository = projectRepository;
            _identityService = identityService;
            _tagService = tagService;
            _nonQueryRepository = nonQueryRepository;
            _fileManager = fileManager;
            _nonQuery = nonQueryRepository1;
        }

        public async Task<Result<Project>> CreateAsync(Project project, IEnumerable<string> memberIds, IEnumerable<string> tagNames, IEnumerable<IFormFile> images)
        {
            project.CreatedAt = DateTime.UtcNow;
            project.UpdatedAt = DateTime.UtcNow;

            var updatedMember = false;
            foreach (var memberId in memberIds.Distinct())
            {
                var user = await _identityService.GetUserByIdAsync(memberId);
                if (user != null)
                {
                    updatedMember = (await _identityService.AddProjectToMemberAsync(user, project)).Success;
                }
            }

            foreach (var image in images)
            {
                project.ImagePaths.Add(new ImagePath
                {
                    Url = await _fileManager.SaveImageAsync(project.CreatorId, image)
                });
            }

            var tagResult = await _tagService.CreateManyTagsForAsync(project, tagNames.Distinct());
            updatedMember = tagResult.Success ? tagResult.Success : updatedMember;

            if (updatedMember)
            {
                return new Result<Project>
                {
                    Success = true,
                    Data = project
                };
            }

            var result = await _nonQueryRepository.CreateAsync(project);
            return new Result<Project>
            {
                Success = result,
                Data = result ? project : null,
                Errors = result ? Array.Empty<string>() : new string[] { StaticErrorMessages<Project>.CreateOperationFailed },
            };
        }

        public async Task<Result<Project>> DeleteAsync(Project project)
        {
            var result = await _nonQueryRepository.DeleteAsync(project);
            return new Result<Project>
            {
                Success = result,
                Errors = result ? Array.Empty<string>() :
                    new string[] { StaticErrorMessages<Project>.DeleteOperationFailed }
            };
        }

        public async Task<Project?> GetByIdAsync(Guid id) => await _projectRepository.GetByIdAsync(id);

        public async Task<List<Project>> GetAllAsync(GetAllProjectsFilter filter = null, PaginationFilter pagination = null)
        {
            var queryable = (await _projectRepository.GetAllAsync()).AsQueryable();

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

        public async Task<Result<Project>> UpdateAsync(Project project, IEnumerable<string> memberIds, IEnumerable<string> tagNames)
        {
            memberIds = memberIds.Distinct();

            project.UpdatedAt = DateTime.UtcNow;

            await _tagService.UpdateTagsAsync(project, tagNames.Distinct());

            var users = await _identityService.GetUsersAsync();
            var usersToRemove = project.Members.Where(member => !memberIds.Contains(member.Id)).ToList();
            var usersToAdd = users.Where(user => memberIds.Contains(user.Id)).ToList();

            var failed = new List<string>();
            var updatedMembers = false;

            foreach (var user in usersToRemove)
            {
                var removingResult = await _identityService.RemoveProjectFromMemberAsync(user, project);
                if (!removingResult.Success)
                {
                    failed.Add($"Failed to remove: {user.UserName} with {user.Id}");
                    continue;
                }

                updatedMembers = true;
            }

            foreach (var user in usersToAdd)
            {
                var addResult = await _identityService.AddProjectToMemberAsync(user, project);
                if (!addResult.Success)
                {
                    failed.Add($"Failed to add: {user.UserName} with {user.Id}");
                    continue;
                }
                updatedMembers = true;
            }

            var result = await _nonQueryRepository.UpdateAsync(project);
            return new Result<Project>
            {
                Success = result,
                Data = result ? project : null,
                Errors = result ? Array.Empty<string>() : new string[] { StaticErrorMessages<Project>.CreateOperationFailed },
            };
        }

        private static IQueryable<Project> AddFiltersOnQuery(GetAllProjectsFilter filter, IQueryable<Project> queryable)
        {
            if (!string.IsNullOrEmpty(filter.CreatorId))
            {
                queryable = queryable.Where(x => x.CreatorId == filter.CreatorId);
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
            if (filter.Tags.Count != 0)
            {
                queryable = queryable.Where(x => filter.Tags.All(y => x.Tags.Select(z => z.Name).Contains(y)));
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