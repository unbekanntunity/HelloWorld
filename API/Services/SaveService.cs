using API.Contracts;
using API.Domain;
using API.Domain.Database;
using API.Domain.Database.Interfaces;
using API.Repositories;
using System.Xml;

namespace API.Services
{
    public class SaveService<T> : ISaveService<T> where T : class, ISavable
    {
        private readonly INonQueryRepository<T> _nonQueryRepository;

        public SaveService(INonQueryRepository<T> nonQueryRepository)
        {
            _nonQueryRepository = nonQueryRepository;
        }

        public async Task<Result<T>> SaveAsync(T item, User user)
        {
            var existingUser = item.SavedBy.FirstOrDefault(x => x.Id == user.Id);
            if (existingUser != null)
            {
                item.SavedBy.Remove(existingUser);
            }
            else
            {
                item.SavedBy.Add(user);
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
