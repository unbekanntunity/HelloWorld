namespace HelloWorldAPI.Repositories
{
    public interface INonQueryRepository<T> where T : class
    {
        Task<bool> CreateAsync(T item);
        Task<bool> UpdateAsync(T item);
        Task<bool> DeleteAsync(T item);
        Task<bool> DeleteRangeAsync(IEnumerable<T> items);
    }
}
