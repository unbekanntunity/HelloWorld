using API.Data;
using API.Domain.Database;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories
{
    public class PostRepository : IPostRepository
    {
        private readonly DataContext _dataContext;

        public PostRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<List<Post>> GetAllAsync() => await _dataContext.Posts
            .Include(x => x.Tags)
            .Include(x => x.ImagePaths)
            .Include(x => x.UsersLiked)
            .ToListAsync();
        public async Task<Post?> GetByIdAsync(Guid id) => await _dataContext.Posts
            .Include(x => x.Comments)
            .Include(x => x.Creator)
            .Include(x => x.Tags)
            .Include(x => x.UsersLiked)
            .Include(x => x.ImagePaths)
            .FirstOrDefaultAsync(x => x.Id == id);
    }
}
