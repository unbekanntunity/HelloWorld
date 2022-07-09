using API.Contracts;
using API.Domain;
using API.Domain.Database;
using API.Domain.Database.Interfaces;
using API.Domain.Filters;
using API.Extensions;
using API.Repositories;

namespace API.Services
{
    public class TagService : ITagService
    {
        private readonly ITagRepository _tagRepository;
        private readonly INonQueryRepository<Tag> _nonQueryRepository;

        public TagService(ITagRepository tagRepository, INonQueryRepository<Tag> nonQueryRepository)
        {
            _tagRepository = tagRepository;
            _nonQueryRepository = nonQueryRepository;
        }

        public async Task<Result<List<string>>> CreateManyTagsForAsync<T>(T item, IEnumerable<string> tagNames) where T : ITagable
        {
            if (!tagNames.Any())
            {
                return new Result<List<string>>
                {
                    Errors = new string[] { "No tags to create" }
                };
            }

            var property = typeof(Tag).GetProperty($"{typeof(T).Name}s");
            if (property == null)
            {
                return new Result<List<string>>
                {
                    Errors = new string[] { "T is not a valid type." }
                };
            }

            var failedTags = new List<string>();
            var success = true;

            foreach (var tagName in tagNames)
            {
                var tagInDb = await GetByNameAsync(tagName);
                if (tagInDb == null)
                {
                    var newTag = new Tag { Name = tagName };
                    property.SetValue(newTag, new List<T> { item });
                    var tagResult = await CreateAsync(newTag);
                    if (!tagResult.Success)
                    {
                        failedTags.Add($"Failed to remove: {newTag.Name}");
                        success = false;
                    }
                }
                else
                {
                    //"a713e4e1-b729-4ee2-8001-30f6f9be2f9c"
                    var tagItems = (List<T>?)property.GetValue(tagInDb);
                    tagItems ??= new List<T>();
                    var tagResult = await AddItemToTagAsync(tagInDb, tagItems, item);
                    if (!tagResult.Success)
                    {
                        failedTags.Add($"Failed to add: {tagInDb.Name}");
                        success = false;
                    }
                }
            }

            return new Result<List<string>>
            {
                Success = success,
                Data = success ? tagNames.ToList() : null,
                Errors = success ? Array.Empty<string>() : failedTags.ToArray()
            };
        }

        public async Task<Result<Tag>> AddItemToTagAsync<T>(Tag tag, List<T> items, T item) where T : ITagable
        {
            items.Add(item);

            var updated = await _nonQueryRepository.UpdateAsync(tag);
            return new Result<Tag>
            {
                Success = updated,
                Data = updated ? tag : null,
                Errors = new string[] { StaticErrorMessages<Tag>.UpdateOperationFailed }
            };
        }

        public async Task<Result<Tag>> RemoveItemFromTagAsync<T>(Tag tag, List<T> items, T item) where T : ITagable
        {
            items.Remove(item);

            var updated = await _nonQueryRepository.UpdateAsync(tag);
            return new Result<Tag>
            {
                Success = updated,
                Data = updated ? tag : null,
                Errors = new string[] { StaticErrorMessages<Tag>.UpdateOperationFailed }
            };
        }
        public async Task<Tag?> GetByNameAsync(string tagName) => await _tagRepository.GetByNameAsync(tagName);

        public async Task<List<Tag>> GetAllAsync(GetAllTagsFilter filter = null, PaginationFilter pagination = null)
        {
            var queryable = (await _tagRepository.GetAllAsync()).AsQueryable();
            if (pagination == null)
            {
                return await queryable.ToListAsyncSafe();
            }
            if (filter != null)
            {
                queryable = AddFiltersOnQuery(filter, queryable);
            }

            var skip = (pagination.PageNumber - 1) * pagination.PageSize;
            return await queryable.Skip(skip).Take(pagination.PageSize).ToListAsyncSafe();
        }

        public async Task<Result<Tag>> CreateAsync(Tag tag)
        {
            var created = await _nonQueryRepository.CreateAsync(tag);
            return new Result<Tag>
            {
                Success = created,
                Data = created ? tag : null,
                Errors = created ? Array.Empty<string>() : new string[] { StaticErrorMessages<Tag>.CreateOperationFailed }
            };
        }

        public async Task<Result<T>> UpdateTagsAsync<T>(T item, IEnumerable<string> newTags) where T : ITagable
        {
            if (!newTags.Any() || !newTags.Except(item.Tags.Select(x => x.Name)).Any() && !item.Tags.Select(x => x.Name).Except(newTags).Any())
            {
                return new Result<T>
                {
                    Success = true,
                    Data = item
                };
            }

            var genericProperties = typeof(Tag).GetProperties().Where(x => x.PropertyType.GenericTypeArguments.Length != 0);
            var property = genericProperties.FirstOrDefault(x => x.PropertyType.GenericTypeArguments[0] == typeof(T));
            if (property == null)
            {
                return new Result<T>
                {
                    Errors = new string[] { $"{typeof(T).Name} is not a valid type." }
                };
            }

            var tags = await _tagRepository.GetAllAsync();
            var tagsToEdit = newTags.Where(x => !item.Tags.Select(y => y.Name).Contains(x)).Select(x => new Tag { Name = x });

            var tagsToRemove = item.Tags.Where(x => !newTags.Contains(x.Name)).ToList();
            var tagsToAssign = tags.Where(x => newTags.Contains(x.Name)).ToList();
            var tagsToCreate = tagsToEdit.Where(x => !tags.Select(z => z.Name).Contains(x.Name)).ToList();

            var failedTags = new List<string>();

            var updated = true;

            for (int i = 0; i < tagsToRemove.Count; i++)
            {
                item.Tags.Remove(tagsToRemove[i]);
                var itemsOfTag = (List<T>)property.GetValue(tagsToRemove[i]);
                var tagResult = await RemoveItemFromTagAsync(tagsToRemove[i], itemsOfTag, item);
                if (!tagResult.Success)
                {
                    failedTags.Add($"Failed to remove: {tagsToRemove[i].Name}");
                    updated = false;
                }
            }

            for (int i = 0; i < tagsToAssign.Count; i++)
            {
                item.Tags.Remove(tagsToAssign[i]);
                var itemsOfTag = (List<T>)property.GetValue(tagsToAssign[i]);
                var tagResult = await AddItemToTagAsync(tagsToAssign[i], itemsOfTag, item);
                if (!tagResult.Success)
                {
                    failedTags.Add($"Failed to add: {tagsToAssign[i].Name}");
                    updated = false;
                }
            }

            for (int i = 0; i < tagsToCreate.Count; i++)
            {
                item.Tags.Remove(tagsToCreate[i]);
                var itemsOfTag = (List<T>)property.GetValue(tagsToCreate[i]);
                itemsOfTag?.Add(item);
                var tagResult = await CreateAsync(tagsToCreate[i]);
                if (!tagResult.Success)
                {
                    failedTags.Add($"Failed to create: {tagsToCreate[i].Name}");
                    updated = false;
                }
            }

            return new Result<T>()
            {
                Success = updated,
                Data = updated ? item : default,
                Errors = updated ? Array.Empty<string>() : failedTags.ToArray()
            };
        }

        private static IQueryable<Tag> AddFiltersOnQuery(GetAllTagsFilter filter, IQueryable<Tag> queryable)
        {
            if (!string.IsNullOrEmpty(filter.tagName))
            {
                queryable = queryable.Where(x => x.Name.StartsWith(filter.tagName));
            }

            return queryable;
        }
    }
}
