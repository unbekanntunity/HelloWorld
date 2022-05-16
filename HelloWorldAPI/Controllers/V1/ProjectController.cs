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
    public class ProjectController : Controller
    {
        private readonly IProjectService _projectService;
        private readonly IUriService _uriService;
        private readonly IIdentityService _identityService;
        private readonly IRateableService<Project> _rateableService;

        public ProjectController(IProjectService projectService, IUriService uriService, IIdentityService identityService, IRateableService<Project> rateableService)
        {
            _projectService = projectService;
            _uriService = uriService;
            _identityService = identityService;
            _rateableService = rateableService;
        }

        [HttpPost(ApiRoutes.Project.Create)]
        public async Task<IActionResult> Create([FromBody] CreateProjectRequest request)
        {
            var project = new Project
            {
                CreatorId = HttpContext.GetUserId(),
                Desciption = request.Desciption,
                Title = request.Title,
            };

            var result = await _projectService.CreateAsync(project, request.Members, request.TagNames);
            if (!result.Success)
            {
                return BadRequest(result.Errors);
            }

            var response = result.Data.ToResponse();
            var location = _uriService.GetUri(ApiRoutes.Project.Get, result.Data.Id.ToString());

            return Created(location, response);
        }

        [HttpDelete(ApiRoutes.Project.Delete)]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var project = await _projectService.GetByIdAsync(id);
            if (project == null)
            {
                return NotFound();
            }
            if (project.CreatorId != HttpContext.GetUserId() && !HttpContext.HasRole("ContentAdmin"))
            {
                return BadRequest(StaticErrorMessages.PermissionDenied);
            }

            var result = await _projectService.DeleteAsync(project);
            return result.Success ? NoContent() : BadRequest(result);
        }


        [HttpGet(ApiRoutes.Project.Get)]
        public async Task<IActionResult> Get([FromRoute] Guid id)
        {
            var project = await _projectService.GetByIdAsync(id);
            if (project == null)
            {
                return NotFound();
            }

            var response = project.ToResponse();
            return project != null ? Ok(new Response<ProjectResponse>(response)) : NotFound();
        }

        [HttpGet(ApiRoutes.Project.GetAll)]
        public async Task<IActionResult> GetAll([FromQuery] GetAllProjectsFilter filter, [FromQuery] PaginationFilter pagination)
        {
            var projects = await _projectService.GetAllAsync(filter, pagination);
            var responses = projects.Select(x => x.ToPartialResponse()).ToList();

            if (pagination == null || pagination.PageNumber < 1 || pagination.PageSize < 1)
            {
                return Ok(new PagedResponse<PartialProjectResponse>(responses));
            }

            var paginationResponse = PaginationHelpers.CreatePaginatedResponse(_uriService, pagination, responses);
            return Ok(paginationResponse);
        }

        [HttpPatch(ApiRoutes.Project.Update)]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateProjectRequest request)
        {
            var project = await _projectService.GetByIdAsync(id);
            if (project == null)
            {
                return NotFound();
            }
            if (project.CreatorId != HttpContext.GetUserId())
            {
                return BadRequest(StaticErrorMessages.PermissionDenied);
            }

            var result = await _projectService.UpdateAsync(project, request.MemberIds, request.Tags);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            var response = result.Data.ToResponse();
            return Ok(new Response<ProjectResponse>(response));
        }

        [HttpPatch(ApiRoutes.Project.UpdateRating)]
        public async Task<IActionResult> UpdateRating([FromRoute] Guid id)
        {
            var article = await _projectService.GetByIdAsync(id);
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
            return Ok(new Response<ProjectResponse>(response));
        }
    }
}
