using HelloWorldAPI.Domain.Filters;

namespace HelloWorldAPI.Services
{
    public interface IUriService
    {
        Uri GetUri(string rawRoute, string id);

        Uri GetAllUri(string rawRoute, PaginationFilter pagination = null);
    }
}
