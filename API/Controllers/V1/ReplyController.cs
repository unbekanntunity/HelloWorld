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
    public class ReplyController : Controller
    {
        private readonly IArticleService _articleService;
        private readonly ICommentService _commentService;
        private readonly IIdentityService _identityService;
        private readonly IReplyService _replyService;
        private readonly IRateableService<Reply> _rateableService;
        private readonly IUriService _uriService;
        private readonly INonQueryRepository<Reply> _nonQueryRepository;

        public ReplyController(IReplyService replyService, IUriService uriService, ICommentService commentService, IArticleService articleService, IRateableService<Reply> rateableService, IIdentityService identityService, INonQueryRepository<Reply> nonQueryRepository)
        {
            _replyService = replyService;
            _uriService = uriService;
            _commentService = commentService;
            _articleService = articleService;
            _rateableService = rateableService;
            _identityService = identityService;
            _nonQueryRepository = nonQueryRepository;
        }

        [HttpPost(ApiRoutes.Reply.CreateOnArticle)]
        public async Task<IActionResult> CreateOnArticle([FromRoute] Guid id, [FromBody] CreateReplyRequest request)
        {
            var article = await _articleService.GetByIdAsync(id);
            if (article == null)
            {
                return NotFound();
            }

            var reply = new Reply
            {
                CreatorId = HttpContext.GetUserId(),
                Content = request.Content
            };

            var result = await _replyService.AddReplyForArticleAsync(article, reply);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            var response = result.Data.ToResponse();
            var location = _uriService.GetUri(ApiRoutes.Reply.Get, result.Data.Id.ToString());
            return Created(location, new Response<ReplyResponse>(response));
        }

        [HttpPost(ApiRoutes.Reply.CreateOnComment)]
        public async Task<IActionResult> CreateOnComment([FromRoute] Guid id, [FromBody] CreateReplyRequest request)
        {
            var comment = await _commentService.GetByIdAsync(id);
            if (comment == null)
            {
                return NotFound();
            }

            var reply = new Reply
            {
                CreatorId = HttpContext.GetUserId(),
                Content = request.Content
            };

            var result = await _replyService.AddReplyForCommentAsync(comment, reply);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            var response = result.Data.ToResponse();
            var location = _uriService.GetUri(ApiRoutes.Reply.Get, result.Data.Id.ToString());
            return Created(location, new Response<ReplyResponse>(response));
        }

        [HttpPost(ApiRoutes.Reply.CreateOnReply)]
        public async Task<IActionResult> CreateOnReply([FromRoute] Guid id, [FromBody] CreateReplyRequest request)
        {
            var repliedOn = await _replyService.GetByIdAsync(id);
            if (repliedOn == null)
            {
                return NotFound();
            }

            var reply = new Reply
            {
                CreatorId = HttpContext.GetUserId(),
                Content = request.Content
            };

            var result = await _replyService.AddReplyForReplyAsync(repliedOn, reply);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            var response = result.Data.ToResponse();
            var location = _uriService.GetUri(ApiRoutes.Reply.Get, result.Data.Id.ToString());
            return Created(location, new Response<ReplyResponse>(response));
        }

        [HttpDelete(ApiRoutes.Reply.Delete)]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var existingReply = await _replyService.GetByIdAsync(id);
            if (existingReply == null)
            {
                return NotFound();
            }

            if (existingReply.CreatorId != HttpContext.GetUserId() && !HttpContext.HasRole("ContentAdmin"))
            {
                return Unauthorized();
            }

            var result = await _replyService.DeleteAsync(existingReply);
            return result.Success ? NoContent() : BadRequest(result);
        }

        [HttpDelete(ApiRoutes.Reply.DeleteAll)]
        public async Task<IActionResult> DeleteAll()
        {
            if (!HttpContext.HasRole("ContentAdmin"))
            {
                return Unauthorized();
            }

            var replies = await _replyService.GetAllAsync();
            await _nonQueryRepository.DeleteRangeAsync(replies);

            return NoContent();
        }

        [HttpGet(ApiRoutes.Reply.Get)]
        public async Task<IActionResult> Get([FromRoute] Guid id)
        {
            var reply = await _replyService.GetByIdAsync(id);
            if (reply == null)
            {
                return NotFound();
            }
            var response = reply.ToResponse();
            return reply != null ? Ok(new Response<ReplyResponse>(response)) : NotFound();
        }

        [HttpGet(ApiRoutes.Reply.GetAll)]
        public async Task<IActionResult> GetAll([FromQuery] GetAllRepliesFilter filter, [FromQuery] PaginationFilter pagination)
        {
            var replies = await _replyService.GetAllAsync(filter, pagination);
            var responses = replies.Select(x => x.ToPartialResponse()).ToList();

            if (pagination == null || pagination.PageSize < 1 || pagination.PageNumber < 1)
            {
                return Ok(new PagedResponse<PartialReplyResponse>(responses));
            }

            var paginationResponse = PaginationHelpers.CreatePaginatedResponse(_uriService, ApiRoutes.Reply.GetAll, pagination, responses);
            return Ok(paginationResponse);
        }

        [HttpPatch(ApiRoutes.Reply.Update)]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateReplyRequest request)
        {
            var existingReply = await _replyService.GetByIdAsync(id);
            if (existingReply == null)
            {
                return NotFound();
            }

            if (existingReply.CreatorId != HttpContext.GetUserId())
            {
                return Unauthorized(StaticErrorMessages.PermissionDenied);
            }

            existingReply.Content = request.Content;
            var result = await _replyService.UpdateAsync(existingReply);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            var response = result.Data.ToResponse();
            return Ok(new Response<ReplyResponse>(response));
        }

        [HttpPatch(ApiRoutes.Reply.UpdateRating)]
        public async Task<IActionResult> UpdateRating([FromRoute] Guid id)
        {
            var existingReply = await _replyService.GetByIdAsync(id);
            var user = await _identityService.GetUserByIdAsync(HttpContext.GetUserId());
            if (existingReply == null || user == null)
            {
                return NotFound();
            }

            var result = await _rateableService.UpdateRatingAsync(existingReply, user);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            var response = result.Data.ToResponse();
            return Ok(new Response<ReplyResponse>(response));
        }
    }
}
