using API.Domain.Filters;

namespace API.Services
{
    public interface IUriService
    {
        Uri GetUri(string rawRoute, string id);
        Uri GetAllUri(string rawRoute, PaginationFilter pagination = null);
        string ConvertPathToUrl(string relativePath);
        List<string> ConvertPathsToUrls(IEnumerable<string> relativePath);
    }
}
