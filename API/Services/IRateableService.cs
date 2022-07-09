using API.Domain;
using API.Domain.Database;
using API.Domain.Database.Interfaces;

namespace API.Services
{
    public interface IRateableService<T> where T : class, IRateable
    {
        Task<Result<T>> UpdateRatingAsync(T item, User user);
    }
}
