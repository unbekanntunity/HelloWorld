using HelloWorldAPI.Contracts.V1;
using HelloWorldAPI.Contracts.V1.Responses;
using HelloWorldAPI.Domain.Filters;
using HelloWorldAPI.Extensions;
using HelloWorldAPI.Helpers;
using HelloWorldAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace HelloWorldAPI.Controllers.V1
{
    public class TagController : Controller
    {
        private readonly ITagService _tagService;
        private readonly IUriService _uriService;

        public TagController(ITagService tagService, IUriService uriService)
        {
            _tagService = tagService;
            _uriService = uriService;
        }

        [HttpGet(ApiRoutes.Tag.GetAll)]
        public async Task<IActionResult> GetAll(PaginationFilter pagination)
        {
            var tags = await _tagService.GetAllAsync(pagination);
            var responses = tags.Select(x => x.ToResponse()).ToList();

            if (pagination == null || pagination.PageSize < 1 || pagination.PageNumber < 1)
            {
                return Ok(new PagedResponse<TagResponse>(responses));
            }

            var paginationResponse = PaginationHelpers.CreatePaginatedResponse(_uriService, pagination, responses);
            return Ok(paginationResponse);
        }
    }
}
