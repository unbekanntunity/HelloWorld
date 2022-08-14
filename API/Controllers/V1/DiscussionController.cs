using API.Contracts;
using API.Contracts.V1;
using API.Contracts.V1.Requests;
using API.Contracts.V1.Responses;
using API.Domain.Database;
using API.Domain.Filters;
using API.Extensions;
using API.Helpers;
using API.Repositories;
using API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.V1
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class DiscussionController : Controller
    {
        private readonly IUriService _uriService;
        private readonly IDiscussionService _discussionService;
        private readonly IFileManager _fileManager;
        private readonly IIdentityService _identityService;
        private readonly IRateableService<Discussion> _rateableService;
        private readonly INonQueryRepository<Discussion> _nonQueryRepository;
        private readonly ISaveService<Discussion> _saveService;

        public DiscussionController(IDiscussionService discussionService, IUriService uriService, IRateableService<Discussion> rateableService, IIdentityService identityService, IFileManager fileManager, INonQueryRepository<Discussion> nonQueryRepository, ISaveService<Discussion> saveService)
        {
            _discussionService = discussionService;
            _uriService = uriService;
            _rateableService = rateableService;
            _identityService = identityService;
            _fileManager = fileManager;
            _nonQueryRepository = nonQueryRepository;
            _saveService = saveService;
        }

        [HttpPost(ApiRoutes.Discussion.Create)]
        public async Task<IActionResult> Create([FromForm] CreateDiscussionRequest request)
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
            var responses = discussions.Select(x => x.ToPartialResponse()).ToList();
            if (pagination == null || pagination.PageNumber < 1 || pagination.PageSize < 1)
            {
                return Ok(new PagedResponse<PartialDiscussionResponse>(responses));
            }

            var paginationReponse = PaginationHelpers.CreatePaginatedResponse(_uriService, ApiRoutes.Discussion.GetAll, pagination, responses);
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
                return NotFound();
            }

            if (existingDiscussion.CreatorId != HttpContext.GetUserId() && !HttpContext.HasRole("ContentAdmin"))
            {
                return Unauthorized(StaticErrorMessages.PermissionDenied);
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
                return NotFound();
            }

            if (existingDiscussion.CreatorId != HttpContext.GetUserId() && !HttpContext.HasRole("ContentAdmin"))
            {
                return Unauthorized(StaticErrorMessages.PermissionDenied);
            }

            var result = await _discussionService.DeleteAsync(id);
            return result.Success ? NoContent() : BadRequest(result);
        }

        [HttpDelete(ApiRoutes.Discussion.DeleteAll)]
        public async Task<IActionResult> DeleteAll()
        {
            if (!HttpContext.HasRole("ContentAdmin"))
            {
                return Unauthorized();
            }

            var discussions = await _discussionService.GetAllAsync();
            var result = await _nonQueryRepository.DeleteRangeAsync(discussions);
            return result ? NoContent() : BadRequest();
        }

        [HttpPatch(ApiRoutes.Discussion.UpdateRating)]
        public async Task<IActionResult> UpdateRating([FromRoute] Guid id)
        {
            var discussion = await _discussionService.GetByIdAsync(id);
            var user = await _identityService.GetUserByIdAsync(HttpContext.GetUserId());
            if (discussion == null || user == null)
            {
                return NotFound();
            }

            var result = await _rateableService.UpdateRatingAsync(discussion, user);
            var response = result.Data.ToResponse();
            return Ok(new Response<DiscussionResponse>(response));
        }

        [HttpPatch(ApiRoutes.Discussion.UpdateSave)]
        public async Task<IActionResult> UpdateSave([FromRoute] Guid id)
        {
            var post = await _discussionService.GetByIdAsync(id);
            var user = await _identityService.GetUserByIdAsync(HttpContext.GetUserId());
            if (post == null || user == null)
            {
                return NotFound();
            }
            var result = await _saveService.SaveAsync(post, user);

            var newUser = await _identityService.GetUserByIdAsync(HttpContext.GetUserId());
            return Ok(new Response<List<DiscussionResponse>>(newUser.SavedDiscussions.Select(x => x.ToResponse()).ToList()));
        }
    }
}
