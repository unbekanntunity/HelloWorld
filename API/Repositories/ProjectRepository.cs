using API.Data;
using API.Domain.Database;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories
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
                .Include(x => x.UsersLiked)
                .Include(x => x.Tags)
                .Include(x => x.ImagePaths)
                .ToListAsync();

        public async Task<Project?> GetByIdAsync(Guid id) => await _dataContext.Projects
                .Include(x => x.Creator)
                .Include(x => x.UsersLiked)
                .Include(x => x.Tags)
                .FirstOrDefaultAsync(x => x.Id == id);
    }
}
