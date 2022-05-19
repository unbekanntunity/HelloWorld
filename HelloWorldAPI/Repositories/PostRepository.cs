using HelloWorldAPI.Data;
using HelloWorldAPI.Domain.Database;
using Microsoft.EntityFrameworkCore;

namespace HelloWorldAPI.Repositories
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
            .ToListAsync();

        public async Task<Post?> GetByIdAsync(Guid id) => await _dataContext.Posts
            .Include(x => x.Comments)
            .Include(x => x.Creator)
            .Include(x => x.Tags)
            .Include(x => x.UserLiked)
            .FirstOrDefaultAsync(x => x.Id == id);
    }
}
