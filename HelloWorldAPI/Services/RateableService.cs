using HelloWorldAPI.Contracts;
using HelloWorldAPI.Domain;
using HelloWorldAPI.Domain.Database;
using HelloWorldAPI.Repositories;

namespace HelloWorldAPI.Services
{
    public class RateableService<T> : IRateableService<T> where T : class, IRateable
    {
        private readonly INonQueryRepository<T> _nonQueryRepository;

        public RateableService(INonQueryRepository<T> nonQueryRepository)
        {
            _nonQueryRepository = nonQueryRepository;
        }

        public async Task<Result<T>> UpdateRatingAsync(T item, User user)
        {
            if (item.UserLiked.Contains(user))
            {
                item.UserLiked.Remove(user);
            }
            else
            {
                item.UserLiked.Add(user);
            }

            var updated = await _nonQueryRepository.UpdateAsync(item);
            return new Result<T>
            {
                Success = updated,
                Data = updated ? item : null,
                Errors = updated ? Array.Empty<string>() :
                    new string[] { StaticErrorMessages<T>.DeleteOperationFailed }
            };
        }
    }
}
