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
    public class ArticleController : Controller
    {
        private readonly IArticleService _articleService;
        private readonly IIdentityService _identityService;
        private readonly IUriService _uriService;
        private readonly IRateableService<Article> _rateableService;

        public ArticleController(IArticleService articleService, IUriService uriService, IIdentityService identityService)
        {
            _articleService = articleService;
            _uriService = uriService;
            _identityService = identityService;
        }

        [HttpPost(ApiRoutes.Article.Create)]
        public async Task<IActionResult> Create([FromRoute] Guid discussionId, [FromBody] CreateArticleRequest request)
        {
            var article = new Article
            {
                CreatorId = HttpContext.GetUserId(),
                Content = request.Content
            };

            var result = await _articleService.CreateInDiscussionAsync(discussionId, article);
            if (!result.Success)
            {
                return BadRequest(result.Errors);
            }

            var response = result.Data.ToResponse();
            var locationUri = _uriService.GetUri(ApiRoutes.Article.Get, response.Id.ToString());
            return Created(locationUri, new Response<ArticleResponse>(response));
        }

        [HttpDelete(ApiRoutes.Article.Delete)]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var existingArticle = await _articleService.GetByIdAsync(id);
            if (existingArticle == null)
            {
                return BadRequest(StaticErrorMessages<Article>.NotFound);
            }

            if (existingArticle.CreatorId != HttpContext.GetUserId() && !HttpContext.HasRole("ContentAdmin"))
            {
                return BadRequest(StaticErrorMessages.PermissionDenied);
            }

            var result = await _articleService.DeleteAsync(existingArticle);
            return result.Success ? NoContent() : BadRequest(result);
        }

        [HttpGet(ApiRoutes.Article.Get)]
        public async Task<IActionResult> Get([FromRoute] Guid id)
        {
            var article = await _articleService.GetByIdAsync(id);
            var response = article.ToResponse();
            return response != null ? Ok(new Response<ArticleResponse>(response)) : NotFound();
        }

        [HttpGet(ApiRoutes.Article.GetAll)]
        public async Task<IActionResult> GetAll([FromQuery] GetAllArticlesFilter filter, [FromQuery] PaginationFilter pagination)
        {
            var articles = await _articleService.GetAllAsync(filter, pagination);
            var responses = articles.Select(article => article.ToResponse()).ToList();

            if (pagination == null || pagination.PageNumber < 1 || pagination.PageSize < 1)
            {
                return Ok(new PagedResponse<ArticleResponse>(responses));
            }

            var paginationResponse = PaginationHelpers.CreatePaginatedResponse(_uriService, pagination, responses);
            return Ok(paginationResponse);
        }

        [HttpPatch(ApiRoutes.Article.Update)]
        public async Task<IActionResult> Update([FromRoute] Guid articleId, [FromBody] UpdateArticleRequest request)
        {
            var existingArticle = await _articleService.GetByIdAsync(articleId);
            if (existingArticle == null)
            {
                return NotFound();
            }

            if (existingArticle.CreatorId != HttpContext.GetUserId())
            {
                return BadRequest(StaticErrorMessages.PermissionDenied);
            }

            existingArticle.Content = request.Content;
            var result = await _articleService.UpdateAsync(existingArticle);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            var response = result.Data.ToResponse();
            return Ok(new Response<ArticleResponse>(response));
        }

        [HttpPatch(ApiRoutes.Article.UpdateRating)]
        public async Task<IActionResult> UpdateRating([FromRoute] Guid id)
        {
            var article = await _articleService.GetByIdAsync(id);
            if (article == null)
            {
                return NotFound();
            }

            var user = await _identityService.GetUserByIdAsync(HttpContext.GetUserId());
            if (user == null)
            {
                return NotFound();
            }

            var result = await _rateableService.UpdateRatingAsync(article, user);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            var response = result.Data.ToResponse();
            return Ok(new Response<ArticleResponse>(response));
        }
    }
}
