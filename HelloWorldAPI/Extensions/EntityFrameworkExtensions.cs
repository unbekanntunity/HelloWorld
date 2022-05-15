using Microsoft.EntityFrameworkCore;

namespace HelloWorldAPI.Extensions
{
    public static class EntityFrameworkExtensions
    {
        public static Task<List<TSource>> ToListAsyncSafe<TSource>(this IQueryable<TSource> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (source is not IAsyncEnumerable<TSource>)
            {
                return Task.FromResult(source.ToList());
            }
            return source.ToListAsync();
        }

        public static void Clear<T>(this DbSet<T> dbSet) where T : class
        {
            dbSet.RemoveRange(dbSet);
        }
    }
}
