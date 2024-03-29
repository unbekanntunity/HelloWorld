﻿using API.Contracts;
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
        private readonly ISaveService<Post> _saveService;
        private readonly IFileManager _fileManager;

        public PostController(IPostService postService, IUriService uriService, IIdentityService identityService, IRateableService<Post> rateableService, IFileManager fileManager, ISaveService<Post> saveService)
        {
            _postService = postService;
            _uriService = uriService;
            _identityService = identityService;
            _rateableService = rateableService;
            _fileManager = fileManager;
            _saveService = saveService;
        }


        [HttpPost(ApiRoutes.Post.Create)]
        public async Task<IActionResult> Create([FromForm] CreatePostRequest request)
        {
            var post = new Post
            {
                CreatorId = HttpContext.GetUserId(),
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

        [HttpDelete(ApiRoutes.Post.Delete)]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            try
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
            catch (Exception e)
            {

                throw;
            }
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

        [HttpGet(ApiRoutes.Post.GetSaved)]
        public async Task<IActionResult> GetSaved([FromRoute] Guid id)
        {
            var user = await _identityService.GetUserByIdAsync(HttpContext.GetUserId());
            if (user == null)
            {
                return NotFound();
            }

            var post = await _postService.GetByIdAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            var result = await _saveService.SaveAsync(post, user);
            var response = result.Data.ToResponse(_fileManager);
            return Ok(new Response<PostResponse>(response));
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

        [HttpPatch(ApiRoutes.Post.UpdateSave)]
        public async Task<IActionResult> UpdateSave([FromRoute] Guid id)
        {
            var post = await _postService.GetByIdAsync(id);
            var user = await _identityService.GetUserByIdAsync(HttpContext.GetUserId());
            if (post == null || user == null)
            {
                return NotFound();
            }
            var result = await _saveService.SaveAsync(post, user);

            var newUser = await _identityService.GetUserByIdAsync(HttpContext.GetUserId());
            return Ok(new Response<List<PostResponse>>(newUser.SavedPosts.Select(x => x.ToResponse(_fileManager)).ToList()));
        }
    }
}
