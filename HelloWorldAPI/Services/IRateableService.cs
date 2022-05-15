using HelloWorldAPI.Domain;
using HelloWorldAPI.Domain.Database;

namespace HelloWorldAPI.Services
{
    public interface IRateableService<T> where T : class, IRateable
    {
        Task<Result<T>> UpdateRatingAsync(T item, User user);
    }
}
