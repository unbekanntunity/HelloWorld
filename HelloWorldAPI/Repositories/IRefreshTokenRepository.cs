using HelloWorldAPI.Domain.Database;
using System.Linq.Expressions;

namespace HelloWorldAPI.Repositories
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken?> GetByPredicateAsync(Expression<Func<RefreshToken, bool>> predicate);
        Task<List<RefreshToken>> GetAllByPredicateAsync(Expression<Func<RefreshToken, bool>> predicate);
    }
}
