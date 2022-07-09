using Microsoft.EntityFrameworkCore;

namespace API.Extensions
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
                Console.WriteLine(source);
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
