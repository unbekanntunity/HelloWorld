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
    public class UserController : Controller
    {
        private readonly IIdentityService _identityService;
        private readonly IUriService _uriService;

        public UserController(IIdentityService identityService, IUriService uriService)
        {
            _identityService = identityService;
            _uriService = uriService;
        }

        [Authorize(Roles = "UserAdmin")]
        [HttpPost(ApiRoutes.Identity.Create)]
        public async Task<IActionResult> Create([FromBody] CreateUserRequest request)
        {
            var newUser = new User
            {
                Description = request.Description,
                Email = request.Email,
                UserName = request.UserName
            };

            var result = HttpContext.HasRole("RootAdmin") ? await _identityService.CreateUserAsync(newUser, request.RoleNames, request.Password) :
                await _identityService.CreateUserAsync(newUser, Array.Empty<string>(), request.Password);

            if (!result.Success)
            {
                return BadRequest(result.Errors);
            }

            var userResponse = await result.Data.ToResponseAsync(_identityService);
            userResponse.Roles = await _identityService.GetAllRolesOfUserAsync(result.Data);

            var locationUri = _uriService.GetUri(ApiRoutes.Identity.Get, result.Data.Id.ToString());
            return Created(locationUri, new Response<UserResponse>(userResponse));
        }

        [HttpDelete(ApiRoutes.Identity.DeleteOwn)]
        public async Task<IActionResult> DeleteOwn()
        {
            var result = await _identityService.DeleteUserAsync(HttpContext.GetUserId());
            return result.Success ? NoContent() : BadRequest(result);
        }

        [Authorize(Roles = "UserAdmin")]
        [HttpDelete(ApiRoutes.Identity.Delete)]
        public async Task<IActionResult> Delete([FromRoute] string id)
        {
            var result = await _identityService.DeleteUserAsync(id);
            return result.Success ? NoContent() : BadRequest(result);
        }

        [HttpGet(ApiRoutes.Identity.Get)]
        public async Task<IActionResult> Get([FromRoute] string id)
        {
            var user = await _identityService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var userResponse = await user.ToResponseAsync(_identityService);
            userResponse.Roles = await _identityService.GetAllRolesOfUserAsync(user);
            return Ok(new Response<UserResponse>(userResponse));
        }

        [HttpGet(ApiRoutes.Identity.GetIdByName)]
        public async Task<IActionResult> GetIdByName([FromRoute] string userName)
        {
            var userId = await _identityService.GetIdByUserNameAsync(userName);
            if (string.IsNullOrEmpty(userId))
            {
                return NotFound();
            }

            return Ok(new Response<string>(userId));
        }

        [HttpGet(ApiRoutes.Identity.GetAll)]
        public async Task<IActionResult> GetAll([FromQuery] GetAllUserFilter filter, [FromQuery] PaginationFilter pagination)
        {
            var users = await _identityService.GetUsersAsync(filter, pagination);
            var responses = new List<PartialUserResponse>();
            foreach (var user in users)
            {
                responses.Add(await user.ToPartialResponseAsync(_identityService));
            }

            if (pagination == null || pagination.PageNumber < 1 || pagination.PageSize < 1)
            {
                return Ok(new PagedResponse<PartialUserResponse>(responses));
            }

            var paginationReponse = PaginationHelpers.CreatePaginatedResponse(_uriService, ApiRoutes.Identity.GetAll, pagination, responses);
            return Ok(paginationReponse);
        }

        [HttpPatch(ApiRoutes.Identity.Update)]
        public async Task<IActionResult> Update([FromBody] UpdateUserRequest request)
        {
            var existingUser = await _identityService.GetUserByIdAsync(HttpContext.GetUserId());

            existingUser.UserName = request.UserName;
            existingUser.Description = request.Description;

            var result = await _identityService.UpdateUserAsync(existingUser, request.TagNames);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            var userResponse = await existingUser.ToResponseAsync(_identityService);
            userResponse.Roles = await _identityService.GetAllRolesOfUserAsync(existingUser);
            return Ok(new Response<UserResponse>(userResponse));
        }

        [HttpPatch(ApiRoutes.Identity.UpdateLogin)]
        public async Task<IActionResult> UpdateLogin([FromRoute] string id, [FromBody] UpdateLoginRequest request)
        {
            var ownUser = await _identityService.OwnUser(HttpContext.GetUserId(), id);
            if (!ownUser && !HttpContext.HasRole("UserAdmin"))
            {
                return Unauthorized(StaticErrorMessages.PermissionDenied);
            }

            var result = await _identityService.UpdateLoginAsync(id, request.Email, request.OldPassword, request.NewPassword);
            return result.Success ? Ok(new Response<string>(result.Data)) : BadRequest(result);
        }
    }
}
