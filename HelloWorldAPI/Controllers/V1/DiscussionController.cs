using HelloWorldAPI.Contracts;
using HelloWorldAPI.Contracts.V1;
using HelloWorldAPI.Contracts.V1.Requests;
using HelloWorldAPI.Contracts.V1.Responses;
using HelloWorldAPI.Domain.Database;
using HelloWorldAPI.Domain.Filters;
using HelloWorldAPI.Extensions;
using HelloWorldAPI.Helpers;
using HelloWorldAPI.Repositories;
using HelloWorldAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HelloWorldAPI.Controllers.V1
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class DiscussionController : Controller
    {
        private readonly IIdentityService _identityService;
        private readonly IUriService _uriService;
        private readonly IDiscussionService _discussionService;

        public DiscussionController(IDiscussionService discussionService, IUriService uriService, IIdentityService identityService)
        {
            _discussionService = discussionService;
            _uriService = uriService;
            _identityService = identityService;
        }

        [HttpPost(ApiRoutes.Discussion.Create)]
        public async Task<IActionResult> Create([FromBody] CreateDiscussionRequest request)
        {
            var discussion = new Discussion
            {
                CreatorId = HttpContext.GetUserId(),
                Title = request.Title,
                StartMessage = request.StartMessage
            };

            var result = await _discussionService.CreateAsync(discussion, request.TagNames);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            var response = result.Data.ToResponse();
            var locationUri = _uriService.GetUri(ApiRoutes.Discussion.Get, result.Data.Id.ToString());
            return Created(locationUri, new Response<DiscussionResponse>(response));
        }

        [HttpGet(ApiRoutes.Discussion.GetAll)]
        public async Task<IActionResult> GetAll([FromQuery] GetAllDiscussionsFilter filter, [FromQuery] PaginationFilter pagination)
        {
            var discussions = await _discussionService.GetAllAsync(filter, pagination);
            var responses = discussions.Select(x => x.ToResponse()).ToList();

            if (pagination == null || pagination.PageNumber < 1 || pagination.PageSize < 1)
            {
                return Ok(new PagedResponse<DiscussionResponse>(responses));
            }

            var paginationReponse = PaginationHelpers.CreatePaginatedResponse(_uriService, pagination, responses);
            return Ok(paginationReponse);
        }

        [HttpGet(ApiRoutes.Discussion.Get)]
        public async Task<IActionResult> Get([FromRoute] Guid id)
        {
            var discussion = await _discussionService.GetByIdAsync(id);
            if (discussion == null)
            {
                return NotFound();
            }

            var response = discussion.ToResponse();
            return discussion != null ? Ok(new Response<DiscussionResponse>(response)) : NotFound();
        }

        [HttpPatch(ApiRoutes.Discussion.Update)]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateDiscussionRequest request)
        {
            var existingDiscussion = await _discussionService.GetByIdAsync(id);

            if (existingDiscussion == null)
            {
                return BadRequest(StaticErrorMessages<Discussion>.NotFound);
            }

            if (existingDiscussion.CreatorId != HttpContext.GetUserId() && !HttpContext.HasRole("ContentAdmin"))
            {
                return BadRequest(StaticErrorMessages.PermissionDenied);
            }

            var result = await _discussionService.UpdateAsync(existingDiscussion, request.Tags);
            var response = result.Data.ToResponse();
            return result.Success ? Ok(new Response<DiscussionResponse>(response)) : BadRequest(result);
        }


        [HttpDelete(ApiRoutes.Discussion.Delete)]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var existingDiscussion = await _discussionService.GetByIdAsync(id);
            if (existingDiscussion == null)
            {
                return NotFound(StaticErrorMessages<Discussion>.NotFound);
            }

            if (existingDiscussion.CreatorId != HttpContext.GetUserId() && !HttpContext.HasRole("ContentAdmin"))
            {
                return BadRequest(StaticErrorMessages.PermissionDenied);
            }

            var result = await _discussionService.DeleteAsync(id);
            return result.Success ? NoContent() : BadRequest(result);
        }
    }
}
