using HelloWorldAPI.Data;
using HelloWorldAPI.Domain.Database;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace HelloWorldAPI.Repositories
{
    public class RefreshtokenRepository : IRefreshTokenRepository
    {
        private readonly DataContext _dataContext;

        public RefreshtokenRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<RefreshToken?> GetByPredicateAsync(Expression<Func<RefreshToken, bool>> predicate) => await _dataContext.RefreshTokens.FirstOrDefaultAsync(predicate);
        public async Task<List<RefreshToken>> GetAllByPredicateAsync(Expression<Func<RefreshToken, bool>> predicate) => await _dataContext.RefreshTokens.Where(predicate).ToListAsync();
    }
}
