using HelloWorldAPI.Contracts;
using HelloWorldAPI.Contracts.V1;
using HelloWorldAPI.Contracts.V1.Requests;
using HelloWorldAPI.Contracts.V1.Responses;
using HelloWorldAPI.Domain.Database;
using HelloWorldAPI.Domain.Filters;
using HelloWorldAPI.Extensions;
using HelloWorldAPI.Helpers;
using HelloWorldAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HelloWorldAPI.Controllers.V1
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PostController : Controller
    {
        private readonly IPostService _postService;
        private readonly IUriService _uriService;
        private readonly IIdentityService _identityService;
        private readonly IRateableService<Post> _rateableService;

        public PostController(IPostService postService, IUriService uriService, IIdentityService identityService, IRateableService<Post> rateableService)
        {
            _postService = postService;
            _uriService = uriService;
            _identityService = identityService;
            _rateableService = rateableService;
        }

        [HttpPost(ApiRoutes.Post.Create)]
        public async Task<IActionResult> Create([FromBody] CreatePostRequest request)
        {
            var post = new Post
            {
                CreatorId = HttpContext.GetUserId(),
                Content = request.Content,
                Title = request.Title,
            };

            var result = await _postService.CreateAsync(post, request.TagNames);
            if (!result.Success)
            {
                return BadRequest(result.Data);
            }

            var response = result.Data.ToResponse();
            var loaction = _uriService.GetUri(ApiRoutes.Post.Get, result.Data.Id.ToString());
            return Created(loaction, new Response<PostResponse>(response));
        }

        [HttpDelete(ApiRoutes.Post.Delete)]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var existingPost = await _postService.GetByIdAsync(id);
            if (existingPost == null)
            {
                return NotFound();
            }
            if (existingPost.CreatorId != HttpContext.GetUserId() && !HttpContext.HasRole("ContentAdmin"))
            {
                return BadRequest(StaticErrorMessages.PermissionDenied);
            }

            var result = await _postService.DeleteAsync(existingPost);
            return result.Success ? NoContent() : BadRequest(result.Errors);
        }


        [HttpGet(ApiRoutes.Post.Get)]
        public async Task<IActionResult> Get([FromRoute] Guid id)
        {
            var post = await _postService.GetByIdAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            var response = post.ToResponse();
            return response != null ? Ok(new Response<PostResponse>(response)) : NotFound();
        }

        [HttpGet(ApiRoutes.Post.GetAll)]
        public async Task<IActionResult> GetAll([FromQuery] GetAllPostsFilters filter, [FromQuery] PaginationFilter pagination)
        {
            var posts = await _postService.GetAllAsync(filter, pagination);
            var responses = posts.Select(x => x.ToResponse()).ToList();
            if (pagination == null || pagination.PageNumber < 1 || pagination.PageSize < 1)
            {
                return Ok(new PagedResponse<PostResponse>(responses));
            }
            var paginationResponse = PaginationHelpers.CreatePaginatedResponse<PostResponse>(_uriService, pagination, responses);
            return Ok(paginationResponse);
        }

        [HttpPatch(ApiRoutes.Post.Update)]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdatePostReqest request)
        {
            var existingPost = await _postService.GetByIdAsync(id);
            if (existingPost == null)
            {
                return NotFound();
            }

            if (existingPost.CreatorId != HttpContext.GetUserId())
            {
                return BadRequest(StaticErrorMessages.PermissionDenied);
            }

            existingPost.Title = request.Title;
            existingPost.Content = request.Content;

            var result = await _postService.UpdateAsync(existingPost, request.TagNames);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            var response = result.Data.ToResponse();
            return Ok(new Response<PostResponse>(response));
        }

        [HttpPatch(ApiRoutes.Post.UpdateRating)]
        public async Task<IActionResult> UpdateRating([FromRoute] Guid id)
        {
            var post = await _postService.GetByIdAsync(id);
            var user = await _identityService.GetUserByIdAsync(HttpContext.GetUserId());
            if (post == null || user == null)
            {
                return NotFound();
            }

            var result = await _rateableService.UpdateRatingAsync(post, user);
            var response = result.Data.ToResponse();
            return Ok(new Response<PostResponse>(response));
        }
    }
}
