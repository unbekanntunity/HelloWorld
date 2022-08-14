using API.Domain;
using API.Domain.Database;
using API.Domain.Database.Interfaces;

namespace API.Services
{
    public interface ISaveService<T> where T : ISavable
    {
        Task<Result<T>> SaveAsync(T item, User user);
    }
}
