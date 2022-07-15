using API.Data;
using API.Domain.Database;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories
{
    public class ReplyRepository : IReplyRepository
    {
        private readonly DataContext _dataContext;

        public ReplyRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<List<Reply>> GetAllAsync() => await _dataContext.Replies
            .Include(x => x.Creator)
            .Include(x => x.RepliedOnComment)
            .Include(x => x.RepliedOnReply)
            .Include(x => x.Replies)
            .ToListAsync();

        public async Task<Reply?> GetByIdAsync(Guid id) => await _dataContext.Replies
            .Include(x => x.Creator)
            .Include(x => x.RepliedOnComment)
            .Include(x => x.RepliedOnReply)
            .Include(x => x.Replies)
            .Include(x => x.UsersLiked)
            .FirstOrDefaultAsync(x => x.Id == id);
    }
}
