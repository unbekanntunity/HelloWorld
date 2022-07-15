using API.Data;
using API.Domain.Database;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories
{
    public class CommentRepository : ICommentRepository
    {
        private readonly DataContext _dataContext;

        public CommentRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<List<Comment>> GetAllAsync() => await _dataContext.Comments
            .Include(x => x.UsersLiked)
            .Include(x => x.Replies)
            .ToListAsync();

        public async Task<Comment?> GetByIdAsync(Guid id) => await _dataContext.Comments
            .Include(x => x.Creator)
            .Include(x => x.Post)
            .Include(x => x.UsersLiked)
            .Include(x => x.Replies)
            .FirstOrDefaultAsync(x => x.Id == id);
    }
}
