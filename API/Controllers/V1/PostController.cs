using API.Contracts;
using API.Contracts.V1;
using API.Contracts.V1.Requests;
using API.Contracts.V1.Responses;
using API.Domain.Database;
using API.Domain.Filters;
using API.Extensions;
using API.Helpers;
using API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.V1
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PostController : Controller
    {
        private readonly IPostService _postService;
        private readonly IUriService _uriService;
        private readonly IIdentityService _identityService;
        private readonly IRateableService<Post> _rateableService;
        private readonly IFileManager _fileManager;

        public PostController(IPostService postService, IUriService uriService, IIdentityService identityService, IRateableService<Post> rateableService, IFileManager fileManager)
        {
            _postService = postService;
            _uriService = uriService;
            _identityService = identityService;
            _rateableService = rateableService;
            _fileManager = fileManager;
        }


        [HttpPost(ApiRoutes.Post.Create)]
        public async Task<IActionResult> Create([FromForm] CreatePostRequest request)
        {
            try
            {
                var id = HttpContext.GetUserId();

                var post = new Post
                {
                    CreatorId = id,
                    Content = request.Content,
                };

                var result = await _postService.CreateAsync(post, request.TagNames, request.RawImages);
                if (!result.Success)
                {
                    return BadRequest(result.Data);
                }

                var response = result.Data.ToResponse(_fileManager);
                var loaction = _uriService.GetUri(ApiRoutes.Post.Get, result.Data.Id.ToString());
                return Created(loaction, new Response<PostResponse>(response));

            }
            catch (Exception e)
            {

                throw;
            }
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
                return Unauthorized(StaticErrorMessages.PermissionDenied);
            }

            var result = await _postService.DeleteAsync(existingPost);
            return result.Success ? NoContent() : BadRequest(result.Errors);
        }

        [HttpDelete(ApiRoutes.Post.DeleteAll)]
        public async Task<IActionResult> DeleteAll()
        {
            var posts = await _postService.GetAllAsync();
            foreach (var post in posts)
            {
                await _postService.DeleteAsync(post);
            }

            return NoContent();
        }


        [HttpGet(ApiRoutes.Post.Get)]
        public async Task<IActionResult> Get([FromRoute] Guid id)
        {
            var post = await _postService.GetByIdAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            var response = post.ToResponse(_fileManager);
            return Ok(new Response<PostResponse>(response));
        }

        [HttpGet(ApiRoutes.Post.GetAll)]
        public async Task<IActionResult> GetAll([FromQuery] GetAllPostsFilters filter, [FromQuery] PaginationFilter pagination)
        {
            var posts = await _postService.GetAllAsync(filter, pagination);
            var responses = posts.Select(x => x.ToPartialResponse(_fileManager)).ToList();
            if (pagination == null || pagination.PageNumber < 1 || pagination.PageSize < 1)
            {
                return Ok(new PagedResponse<PartialPostResponse>(responses));
            }
            var paginationResponse = PaginationHelpers.CreatePaginatedResponse(_uriService, ApiRoutes.Post.GetAll, pagination, responses);
            return Ok(paginationResponse);
        }

        [HttpGet(ApiRoutes.Post.GetAllMinimal)]
        public async Task<IActionResult> GetAllMinimal([FromQuery] GetAllPostsFilters filter, [FromQuery] PaginationFilter pagination)
        {
            var posts = await _postService.GetAllAsync(filter, pagination);
            var responses = posts.Select(x => x.ToMinPostResponse(_fileManager)).ToList();
            if (pagination == null || pagination.PageNumber < 1 || pagination.PageSize < 1)
            {
                return Ok(new PagedResponse<MinimalPostResponse>(responses));
            }
            var paginationResponse = PaginationHelpers.CreatePaginatedResponse(_uriService, ApiRoutes.Post.GetAll, pagination, responses);
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
                return Unauthorized(StaticErrorMessages.PermissionDenied);
            }

            existingPost.Content = request.Content;

            var result = await _postService.UpdateAsync(existingPost, request.TagNames);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            var response = result.Data.ToResponse(_fileManager);
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
            var response = result.Data.ToResponse(_fileManager);
            return Ok(new Response<PostResponse>(response));
        }
    }
}
