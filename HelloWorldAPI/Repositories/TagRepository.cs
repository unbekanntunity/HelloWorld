using HelloWorldAPI.Data;
using HelloWorldAPI.Domain.Database;
using Microsoft.EntityFrameworkCore;

namespace HelloWorldAPI.Repositories
{
    public class TagRepository : ITagRepository
    {
        private readonly DataContext _dataContext;

        public TagRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<List<Tag>> GetAllAsync() => await _dataContext.Tags.ToListAsync();

        public async Task<List<string>> GetAllNamesNoTrackingAsync() => await _dataContext.Tags.AsNoTracking().Select(x => x.Name).ToListAsync();
        public async Task<Tag?> GetByNameAsync(string name)
        {
            return await _dataContext.Tags.Include(x => x.Discussions)
                .FirstOrDefaultAsync(x => x.Name == name);
        }
    }
}
