using HelloWorldAPI.Data;
using HelloWorldAPI.Domain.Database;
using Microsoft.EntityFrameworkCore;

namespace HelloWorldAPI.Repositories
{
    public class ArticleRepository : IArticleRepository
    {
        private readonly DataContext _dataContext;

        public ArticleRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<List<Article>> GetAllAsync()
        {
            return await _dataContext.Articles
                .Include(x => x.Creator)
                .Include(x => x.Discussion)
                .Include(x => x.UserLiked)
                //.Include(x => x.Replies)
                .ToListAsync();
        }

        public async Task<Article?> GetByIdAsync(Guid articleId)
        {
            return await _dataContext.Articles
                .Include(x => x.Creator)
                .Include(x => x.Discussion)
                .Include(x => x.UserLiked)
                //.Include(x => x.Replies)
                .FirstOrDefaultAsync(x => x.Id == articleId);
        }
    }
}
