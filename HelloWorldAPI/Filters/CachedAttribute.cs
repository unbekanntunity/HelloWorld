using Microsoft.AspNetCore.Mvc.Filters;
using System.Text;

namespace HelloWorldAPI.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class CachedAttribute : Attribute, IAsyncActionFilter
    {
        private readonly int _timeToLiveSeconds;

        public CachedAttribute(int timeToLiveSeconds)
        {
            _timeToLiveSeconds = timeToLiveSeconds;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            //var cacheSettings = context.HttpContext.RequestServices.GetRequiredService<RedisCacheSettings>();

            //if (!cacheSettings.Enabled)
            //{
            //    await next();
            //    return;
            //}

            //var cacheService = context.HttpContext.RequestServices.GetRequiredService<IResponseCacheService>();

            //var cacheKey = GenerateCacheKeyFromRequest(context.HttpContext.Request);
            //var cacheResponse = await cacheService.GetCachedResponseAsync(cacheKey);

            //if (!string.IsNullOrEmpty(cacheResponse))
            //{
            //    var contentResult = new ContentResult
            //    {
            //        Content = cacheResponse,
            //        ContentType = "application/json",
            //        StatusCode = 200
            //    };
            //    context.Result = contentResult;
            //    return;
            //}

            //var executedContent = await next();

            //if (executedContent.Result is OkObjectResult objectResult)
            //{
            //    await cacheService.CacheResponseAsync(cacheKey, objectResult.Value, TimeSpan.FromSeconds(_timeToLiveSeconds));
            //}

            await next();
        }

        private static string GenerateCacheKeyFromRequest(HttpRequest request)
        {
            var keyBuilder = new StringBuilder();

            keyBuilder.Append($"{request.Path}");
            foreach (var (key, value) in request.Query.OrderBy(x => x.Key))
            {
                keyBuilder.Append($"|{key}-{value}");
            }

            return keyBuilder.ToString();
        }
    }
}
