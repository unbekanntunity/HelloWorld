using HelloWorldAPI.Domain.Filters;
using Microsoft.AspNetCore.WebUtilities;

namespace HelloWorldAPI.Services
{
    public class UriService : IUriService
    {
        private readonly string _baseUri;

        public UriService(string baseUri)
        {
            _baseUri = baseUri;
        }

        public Uri GetAllUri(string rawRoute, PaginationFilter pagination = null)
        {
            var uri = new Uri(_baseUri + rawRoute);

            if (pagination == null)
            {
                return uri;
            }

            var modifiedUri = QueryHelpers.AddQueryString(uri.ToString(), "pageNumber", pagination.PageNumber.ToString());
            modifiedUri = QueryHelpers.AddQueryString(modifiedUri, "pageSize", pagination.PageSize.ToString());

            return new Uri(modifiedUri);
        }

        public Uri GetUri(string rawRoute, string id) => new(_baseUri + rawRoute.Replace("{id}", id));

        public string ConvertPathToUrl(string relativePath) => new Uri(_baseUri + relativePath).ToString();

        public List<string> ConvertPathsToUrls(IEnumerable<string> relativePath) => relativePath.Select(x => new Uri(_baseUri + x).ToString()).ToList();
    }
}
