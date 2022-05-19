using HelloWorldAPI.Contracts;
using HelloWorldAPI.Contracts.V1;
using HelloWorldAPI.Contracts.V1.Requests;
using HelloWorldAPI.Contracts.V1.Responses;
using HelloWorldAPI.Domain.Database;
using HelloWorldAPI.Domain.Filters;
using HelloWorldAPI.Extensions;
using HelloWorldAPI.Helpers;
using HelloWorldAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace HelloWorldAPI.Controllers.V1
{
    public class ReplyController : Controller
    {
        private readonly IArticleService _articleService;
        private readonly ICommentService _commentService;
        private readonly IReplyService _replyService;
        private readonly IUriService _uriService;

        public ReplyController(IReplyService replyService, IUriService uriService, ICommentService commentService, IArticleService articleService)
        {
            _replyService = replyService;
            _uriService = uriService;
            _commentService = commentService;
            _articleService = articleService;
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

            var result = await _replyService.DeleteAsync(existingReply);
            return result.Success ? NoContent() : BadRequest(result);
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
            var responses = replies.Select(x => x.ToResponse()).ToList();

            if (pagination == null || pagination.PageSize < 1 || pagination.PageNumber < 1)
            {
                return Ok(new PagedResponse<ReplyResponse>(responses));
            }

            var paginationResponse = PaginationHelpers.CreatePaginatedResponse(_uriService, pagination, responses);
            return Ok(paginationResponse);
        }

        [HttpPatch(ApiRoutes.Reply.Update)]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateReplyRequest request)
        {
            var existingReply = await _replyService.GetByIdAsync(id);
            if(existingReply == null)
            {
                return NotFound();
            }

            if(existingReply.CreatorId == HttpContext.GetUserId())
            {
                return Unauthorized(StaticErrorMessages.PermissionDenied);
            }

            existingReply.Content = request.Content;
            var result = await _replyService.UpdateAsync(existingReply);
            if(!result.Success)
            {
                return BadRequest(result);
            }

            var response = result.Data.ToResponse();
            return Ok(response);
        }
    }
}
