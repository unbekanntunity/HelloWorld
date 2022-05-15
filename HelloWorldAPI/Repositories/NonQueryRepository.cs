using HelloWorldAPI.Data;

namespace HelloWorldAPI.Repositories
{
    public class NonQueryRepository<T> : INonQueryRepository<T> where T : class
    {
        private readonly DataContext _dataContext;

        public NonQueryRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<bool> CreateAsync(T item)
        {
            if(item == null)
            {
                return false;
            }

            await _dataContext.AddAsync(item);
            return await _dataContext.SaveChangesAsync() != 0;
        }

        public async Task<bool> DeleteAsync(T item)
        {
            if (item == null)
            {
                return false;
            }

            _dataContext.Remove(item);
            return await _dataContext.SaveChangesAsync() != 0;
        }

        public async Task<bool> DeleteRangeAsync(IEnumerable<T> items)
        {
            _dataContext.RemoveRange(items);
            return await _dataContext.SaveChangesAsync() != 0;
        }

        public async Task<bool> UpdateAsync(T item)
        {
            if (item == null)
            {
                return false;
            }

            _dataContext.Update(item);
            return await _dataContext.SaveChangesAsync() != 0;
        }
    }
}
