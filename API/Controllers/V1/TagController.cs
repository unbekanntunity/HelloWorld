using API.Contracts.V1;
using API.Contracts.V1.Responses;
using API.Domain.Database;
using API.Domain.Filters;
using API.Extensions;
using API.Helpers;
using API.Repositories;
using API.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.V1
{
    public class TagController : Controller
    {
        private readonly ITagService _tagService;
        private readonly IUriService _uriService;
        private readonly INonQueryRepository<Tag> _nonQueryRepository;


        public TagController(ITagService tagService, IUriService uriService, INonQueryRepository<Tag> nonQueryRepository)
        {
            _tagService = tagService;
            _uriService = uriService;
            _nonQueryRepository = nonQueryRepository;
        }

        [HttpGet(ApiRoutes.Tag.GetAll)]
        public async Task<IActionResult> GetAll([FromQuery] GetAllTagsFilter filter, [FromQuery] PaginationFilter pagination)
        {
            var tags = await _tagService.GetAllAsync(filter, pagination);
            var responses = tags.Select(x => x.ToResponse()).ToList();

            if (pagination == null || pagination.PageSize < 1 || pagination.PageNumber < 1)
            {
                return Ok(new PagedResponse<TagResponse>(responses));
            }

            var paginationResponse = PaginationHelpers.CreatePaginatedResponse(_uriService, ApiRoutes.Tag.GetAll, pagination, responses);
            return Ok(paginationResponse);
        }

        [HttpDelete(ApiRoutes.Tag.DeleteAll)]
        public async Task<IActionResult> DeleteAll()
        {
            if(!HttpContext.HasRole("ContentAdmin"))
            {
                return Unauthorized();
            }

            var tags = await _tagService.GetAllAsync();
            await _nonQueryRepository.DeleteRangeAsync(tags);

            return NoContent();
        }
    }
}
