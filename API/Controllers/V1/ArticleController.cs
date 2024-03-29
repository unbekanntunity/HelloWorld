﻿using API.Contracts;
using API.Contracts.V1;
using API.Contracts.V1.Requests;
using API.Contracts.V1.Responses;
using API.Domain.Database;
using API.Domain.Filters;
using API.Extensions;
using API.Helpers;
using API.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.V1
{
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ArticleController : Controller
    {
        private readonly IArticleService _articleService;
        private readonly IDiscussionService _discussionService;
        private readonly IIdentityService _identityService;
        private readonly IUriService _uriService;
        private readonly IRateableService<Article> _rateableService;

        public ArticleController(IArticleService articleService, IUriService uriService, IIdentityService identityService, IDiscussionService discussionService, IRateableService<Article> rateableService)
        {
            _articleService = articleService;
            _uriService = uriService;
            _identityService = identityService;
            _discussionService = discussionService;
            _rateableService = rateableService;
        }

        [HttpPost(ApiRoutes.Article.Create)]
        public async Task<IActionResult> Create([FromRoute] Guid discussionId, [FromBody] CreateArticleRequest request)
        {
            var discussion = await _discussionService.GetByIdAsync(discussionId);
            if (discussion == null)
            {
                return NotFound(StaticErrorMessages<Discussion>.NotFound);
            }

            var article = new Article
            {
                CreatorId = HttpContext.GetUserId(),
                Content = request.Content
            };

            var result = await _articleService.CreateInDiscussionAsync(discussion, article);
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
                return NotFound();
            }

            if (existingArticle.CreatorId != HttpContext.GetUserId() && !HttpContext.HasRole("ContentAdmin"))
            {
                return Unauthorized(StaticErrorMessages.PermissionDenied);
            }

            var result = await _articleService.DeleteAsync(existingArticle);
            return result.Success ? NoContent() : BadRequest(result);
        }

        [HttpGet(ApiRoutes.Article.Get)]
        public async Task<IActionResult> Get([FromRoute] Guid id)
        {
            var article = await _articleService.GetByIdAsync(id);
            if (article == null)
            {
                return NotFound();
            }

            var response = article.ToResponse();
            return Ok(new Response<ArticleResponse>(response));
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

            var paginationResponse = PaginationHelpers.CreatePaginatedResponse(_uriService, ApiRoutes.Article.GetAll, pagination, responses);
            return Ok(paginationResponse);
        }

        [HttpPatch(ApiRoutes.Article.Update)]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateArticleRequest request)
        {
            var existingArticle = await _articleService.GetByIdAsync(id);
            if (existingArticle == null)
            {
                return NotFound();
            }

            if (existingArticle.CreatorId != HttpContext.GetUserId())
            {
                return Unauthorized(StaticErrorMessages.PermissionDenied);
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
            var user = await _identityService.GetUserByIdAsync(HttpContext.GetUserId());
            if (article == null || user == null)
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
