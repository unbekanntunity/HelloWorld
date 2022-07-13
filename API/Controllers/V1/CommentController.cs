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
    public class CommentController : Controller
    {
        private readonly ICommentService _commentService;
        private readonly IIdentityService _identityService;
        private readonly IPostService _postService;
        private readonly IUriService _uriService;
        private readonly IRateableService<Comment> _rateableService;

        public CommentController(ICommentService commentService, IUriService uriService, IIdentityService identityService, IRateableService<Comment> rateableService, IPostService postService)
        {
            _commentService = commentService;
            _uriService = uriService;
            _identityService = identityService;
            _rateableService = rateableService;
            _postService = postService;
        }

        [HttpPost(ApiRoutes.Comment.Create)]
        public async Task<IActionResult> Create([FromRoute] Guid postId, [FromBody] CreateCommentRequest request)
        {
            var post = await _postService.GetByIdAsync(postId);
            if (post == null)
            {
                return NotFound(StaticErrorMessages<Post>.NotFound);
            }

            var comment = new Comment
            {
                CreatorId = HttpContext.GetUserId(),
                Content = request.Content
            };

            var result = await _commentService.CreateInPostAsync(post, comment);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            var response = comment.ToResponse();
            var location = _uriService.GetUri(ApiRoutes.Comment.Get, result.Data.Id.ToString());
            return Created(location, new Response<CommentResponse>(response));
        }

        [HttpDelete(ApiRoutes.Comment.Delete)]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var existingComment = await _commentService.GetByIdAsync(id);
            if (existingComment == null)
            {
                return NotFound();
            }

            if (existingComment.CreatorId != HttpContext.GetUserId() && !HttpContext.HasRole("ContentAdmin"))
            {
                return Unauthorized(StaticErrorMessages.PermissionDenied);
            }

            var result = await _commentService.DeleteAsync(existingComment);
            return result.Success ? NoContent() : BadRequest(result);
        }


        [HttpGet(ApiRoutes.Comment.Get)]
        public async Task<IActionResult> Get([FromRoute] Guid id)
        {
            var comment = await _commentService.GetByIdAsync(id);
            if (comment == null)
            {
                return NotFound();
            }

            var response = comment.ToResponse();
            return Ok(new Response<CommentResponse>(response));
        }

        [HttpGet(ApiRoutes.Comment.GetAll)]
        public async Task<IActionResult> GetAll([FromQuery] GetAllCommentsFilter filter, [FromQuery] PaginationFilter pagination)
        {
            var comments = await _commentService.GetAllAsync(filter, pagination);
            var responses = comments.Select(x => x.ToResponse()).ToList();
            if (pagination == null || pagination.PageNumber < 1 || pagination.PageSize < 1)
            {
                return Ok(new PagedResponse<CommentResponse>(responses));
            }

            var paginationResponse = PaginationHelpers.CreatePaginatedResponse(_uriService, ApiRoutes.Comment.GetAll, pagination, responses);
            return Ok(paginationResponse);
        }

        [HttpPatch(ApiRoutes.Comment.Update)]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateCommentRequest request)
        {
            var existingComment = await _commentService.GetByIdAsync(id);
            if (existingComment == null)
            {
                return NotFound();
            }
            if (existingComment.CreatorId != HttpContext.GetUserId())
            {
                return Unauthorized(StaticErrorMessages.PermissionDenied);
            }

            existingComment.Content = request.Content;
            var result = await _commentService.UpdateAsync(existingComment);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            var response = result.Data.ToResponse();
            return Ok(new Response<CommentResponse>(response));
        }

        [HttpPatch(ApiRoutes.Comment.UpdateRating)]
        public async Task<IActionResult> UpdateRating([FromRoute] Guid id)
        {
            var comment = await _commentService.GetByIdAsync(id);
            if (comment == null)
            {
                return NotFound();
            }

            var user = await _identityService.GetUserByIdAsync(HttpContext.GetUserId());
            if (user == null)
            {
                return NotFound();
            }

            var result = await _rateableService.UpdateRatingAsync(comment, user);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            var response = result.Data.ToResponse();
            return Ok(new Response<CommentResponse>(response));
        }
    }
}
