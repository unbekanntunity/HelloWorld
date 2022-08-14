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
using Newtonsoft.Json;

namespace API.Controllers.V1
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ProjectController : Controller
    {
        private readonly IProjectService _projectService;
        private readonly IUriService _uriService;
        private readonly IIdentityService _identityService;
        private readonly IFileManager _fileManager;
        private readonly IRateableService<Project> _rateableService;
        private readonly INonQueryRepository<Project> _nonQueryRepository;
        private readonly ISaveService<Project> _saveService;

        public ProjectController(IProjectService projectService, IUriService uriService, IIdentityService identityService, IRateableService<Project> rateableService, IFileManager fileManager, INonQueryRepository<Project> nonQueryRepository, ISaveService<Project> saveService)
        {
            _projectService = projectService;
            _uriService = uriService;
            _identityService = identityService;
            _rateableService = rateableService;
            _fileManager = fileManager;
            _nonQueryRepository = nonQueryRepository;
            _saveService = saveService;
        }

        [HttpPost(ApiRoutes.Project.Create)]
        public async Task<IActionResult> Create([FromForm] CreateProjectRequest request)
        {
            var project = new Project
            {
                CreatorId = HttpContext.GetUserId(),
                Desciption = request.Description,
                Title = request.Title,
                Links = request.Links.Select(x => JsonConvert.DeserializeObject<Link>(x)).ToList()
            };

            var result = await _projectService.CreateAsync(project, request.MemberIds, request.TagNames, request.RawImages);
            if (!result.Success)
            {
                return BadRequest(result.Errors);
            }

            var response = result.Data.ToResponse(_fileManager);
            var location = _uriService.GetUri(ApiRoutes.Project.Get, result.Data.Id.ToString());
            return Created(location, new Response<ProjectResponse>(response));
        }

        [HttpDelete(ApiRoutes.Project.Delete)]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            try
            {
                var project = await _projectService.GetByIdAsync(id);
                if (project == null)
                {
                    return NotFound();
                }
                if (project.CreatorId != HttpContext.GetUserId() && !HttpContext.HasRole("ContentAdmin"))
                {
                    return Unauthorized(StaticErrorMessages.PermissionDenied);
                }

                var result = await _projectService.DeleteAsync(project);
                return result.Success ? NoContent() : BadRequest(result);

            }
            catch (Exception e)
            {

                throw;
            }
        }

        [HttpDelete(ApiRoutes.Project.DeleteAll)]
        public async Task<IActionResult> DeleteAll()
        {
            if (!HttpContext.HasRole("ContentAdmin"))
            {
                return Unauthorized();
            }

            var projects = await _projectService.GetAllAsync();
            await _nonQueryRepository.DeleteRangeAsync(projects);

            return NoContent();
        }


        [HttpGet(ApiRoutes.Project.Get)]
        public async Task<IActionResult> Get([FromRoute] Guid id)
        {
            var project = await _projectService.GetByIdAsync(id);
            if (project == null)
            {
                return NotFound();
            }

            var response = project.ToResponse(_fileManager);
            return project != null ? Ok(new Response<ProjectResponse>(response)) : NotFound();
        }

        [HttpGet(ApiRoutes.Project.GetAll)]
        public async Task<IActionResult> GetAll([FromQuery] GetAllProjectsFilter filter, [FromQuery] PaginationFilter pagination)
        {
            var projects = await _projectService.GetAllAsync(filter, pagination);
            var responses = projects.Select(x => x.ToPartialResponse(_fileManager)).ToList();

            if (pagination == null || pagination.PageNumber < 1 || pagination.PageSize < 1)
            {
                return Ok(new PagedResponse<PartialProjectResponse>(responses));
            }

            var paginationResponse = PaginationHelpers.CreatePaginatedResponse(_uriService, ApiRoutes.Project.GetAll, pagination, responses);
            return Ok(paginationResponse);
        }

        [HttpPatch(ApiRoutes.Project.Update)]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromForm] UpdateProjectRequest request)
        {
            var project = await _projectService.GetByIdAsync(id);
            if (project == null)
            {
                return NotFound();
            }
            if (project.CreatorId != HttpContext.GetUserId())
            {
                return Unauthorized(StaticErrorMessages.PermissionDenied);
            }

            project.Title = request.Title;
            project.Desciption = request.Description;
            var result = await _projectService.UpdateAsync(project, request.MemberIds, request.TagNames);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            var response = result.Data.ToResponse(_fileManager);
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

            var response = result.Data.ToResponse(_fileManager);
            return Ok(new Response<ProjectResponse>(response));
        }

        [HttpPatch(ApiRoutes.Project.UpdateSave)]
        public async Task<IActionResult> UpdateSave([FromRoute] Guid id)
        {
            var project = await _projectService.GetByIdAsync(id);
            var user = await _identityService.GetUserByIdAsync(HttpContext.GetUserId());
            if (project == null || user == null)
            {
                return NotFound();
            }
            var result = await _saveService.SaveAsync(project, user);

            var newUser = await _identityService.GetUserByIdAsync(HttpContext.GetUserId());
            return Ok(new Response<List<ProjectResponse>>(newUser.SavedProjects.Select(x => x.ToResponse(_fileManager)).ToList()));
        }
    }
}
