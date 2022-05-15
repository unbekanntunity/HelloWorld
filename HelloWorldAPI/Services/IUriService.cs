using HelloWorldAPI.Domain.Filters;

namespace HelloWorldAPI.Services
{
    public interface IUriService
    {
        Uri GetUri(string rawRoute, string Id);

        Uri GetAllUri(PaginationFilter pagination = null);
    }
}
