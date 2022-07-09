using API.Domain;
using API.Domain.Database;
using API.Domain.Filters;

namespace API.Services
{
    public interface IProjectService
    {
        Task<Result<Project>> CreateAsync(Project project, IEnumerable<string> memberIds, IEnumerable<string> tagNames, IEnumerable<IFormFile> images);
        Task<Result<Project>> DeleteAsync(Project project);
        Task<Result<Project>> UpdateAsync(Project project, IEnumerable<string> memberIds, IEnumerable<string> tagNames);
        Task<Project?> GetByIdAsync(Guid id);
        Task<List<Project>> GetAllAsync(GetAllProjectsFilter filter = null, PaginationFilter pagination = null);
    }
}
