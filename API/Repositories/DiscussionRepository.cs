using API.Data;
using API.Domain.Database;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories
{
    public class DiscussionRepository : IDiscussionRepository
    {
        private readonly DataContext _dataContext;

        public DiscussionRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<List<Discussion>> GetAllAsync()
        {
            return await _dataContext.Discussions
                .Include(x => x.Tags)
                .ToListAsync();
        }

        public async Task<Discussion?> GetByIdAsync(Guid id) => await _dataContext.Discussions
            .Include(x => x.Tags)
            .Include(x => x.Articles)
            .FirstOrDefaultAsync(x => x.Id == id);
    }
}
