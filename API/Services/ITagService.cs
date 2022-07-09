using API.Domain;
using API.Domain.Database;
using API.Domain.Database.Interfaces;
using API.Domain.Filters;

namespace API.Services
{
    public interface ITagService
    {
        Task<Result<Tag>> AddItemToTagAsync<T>(Tag tag, List<T> items, T item) where T : ITagable;
        Task<Result<Tag>> RemoveItemFromTagAsync<T>(Tag tag, List<T> items, T item) where T : ITagable;

        Task<Result<Tag>> CreateAsync(Tag tag);
        Task<Result<List<string>>> CreateManyTagsForAsync<T>(T item, IEnumerable<string> tagNames) where T : ITagable;

        Task<Tag?> GetByNameAsync(string tagName);
        Task<List<Tag>> GetAllAsync(GetAllTagsFilter filter = null, PaginationFilter pagination = null);

        Task<Result<T>> UpdateTagsAsync<T>(T item, IEnumerable<string> newTags) where T : ITagable;
    }
}
