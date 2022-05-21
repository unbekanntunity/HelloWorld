using HelloWorldAPI.Contracts.V1.Responses;
using HelloWorldAPI.Domain.Filters;
using HelloWorldAPI.Services;

namespace HelloWorldAPI.Helpers
{
    public class PaginationHelpers
    {
        internal static PagedResponse<T> CreatePaginatedResponse<T>(IUriService uriService, string rawRoute, PaginationFilter pagination, List<T> response)
        {
            var nextPage = pagination.PageNumber >= 1
                ? uriService.GetAllUri(rawRoute, new PaginationFilter(pagination.PageNumber + 1, pagination.PageSize)).ToString()
                : null;

            var previousPage = pagination.PageNumber - 1 >= 1
                ? uriService.GetAllUri(rawRoute, new PaginationFilter(pagination.PageNumber - 1, pagination.PageSize)).ToString()
                : null;

            var paginationResponse = new PagedResponse<T>(response)
            {
                Data = response,
                PageNumber = pagination.PageNumber >= 1 ? pagination.PageNumber : null,
                PageSize = pagination.PageSize >= 1 ? pagination.PageSize : null,
                NextPage = response.Any() ? nextPage : null,
                PreviousPage = previousPage,
            };
            return paginationResponse;
        }

    }
}
