using HelloWorldAPI.Data;
using HelloWorldAPI.Domain.Database;
using Microsoft.EntityFrameworkCore;

namespace HelloWorldAPI.Repositories
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly DataContext _dataContext;

        public ProjectRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        //TODO Add Profile Pic and Username instead of Id to getallquery
        public async Task<List<Project>> GetAllAsync() => await _dataContext.Projects
                //.Include(x => x.Creator)
                .Include(x => x.UserLiked)
                .Include(x => x.Tags)
                .ToListAsync();

        public async Task<Project?> GetByIdAsync(Guid id) => await _dataContext.Projects
                .Include(x => x.Creator)
                .Include(x => x.UserLiked)
                .Include(x => x.Tags)
                .FirstOrDefaultAsync(x => x.Id == id);
    }
}
