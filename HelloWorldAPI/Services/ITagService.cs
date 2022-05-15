using HelloWorldAPI.Domain;
using HelloWorldAPI.Domain.Database;
using HelloWorldAPI.Domain.Filters;

namespace HelloWorldAPI.Services
{
    public interface ITagService
    {
        Task<Result<Tag>> AddItemToTagAsync<T>(Tag tag, List<T> items, T item);
        Task<Result<Tag>> RemoveItemFromTagAsync<T>(Tag tag, List<T> items, T item);

        Task<Result<Tag>> CreateAsync(Tag tag);
        Task<Result<List<string>>> CreateManyTagsForAsync<T>(T item, IEnumerable<string> tagNames);

        Task<Tag?> GetByNameAsync(string tagName);
        Task<List<Tag>> GetAllAsync(PaginationFilter pagination = null);

        Task<Result<T>> UpdateTagsAsync<T>(T item, List<Tag> itemTags, IEnumerable<string> newTags);   
    }
}
