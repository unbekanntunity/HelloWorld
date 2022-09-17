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
    public class ReportController : Controller
    {
        private readonly IReportService _reportService;
        private readonly IPostService _postService;
        private readonly IDiscussionService _discussionService;
        private readonly IProjectService _projectService;

        private readonly IUriService _uriService;

        public ReportController(IReportService reportService, IUriService uriService, IDiscussionService discussionService, IProjectService projectService, IPostService postService)
        {
            _reportService = reportService;
            _postService = postService;
            _discussionService = discussionService;
            _projectService = projectService;
            _uriService = uriService;
        }

        [HttpPost(ApiRoutes.Report.Create)]
        public async Task<IActionResult> Create(CreateReportRequest request)
        {
            if(request.ContentType != "Post" && request.ContentType != "Discussion" && request.ContentType != "Project")
            {
                return BadRequest();
            }

            var existingPost = await _postService.GetByIdAsync(request.ContentId);
            var existingDiscussion = await _discussionService.GetByIdAsync(request.ContentId);
            var existingProject = await _projectService.GetByIdAsync(request.ContentId);

            if ((request.ContentType == "Post" && existingPost == null) ||
                (request.ContentType == "Discussion" && existingDiscussion == null) ||
                (request.ContentType == "Project" && existingProject == null))
            {
                return NotFound();
            }

            var report = new Report
            {
                CreatorId = HttpContext.GetUserId(),
                Description = request.Description,
                ContentType = request.ContentType,
                ContentId = request.ContentId,
            };

            var result = await _reportService.CreateAsync(report);
            if (!result.Success)
            {
                return BadRequest(result.Data);
            }

            var response = result.Data.ToReponse();
            var loaction = _uriService.GetUri(ApiRoutes.Post.Get, result.Data.Id.ToString());
            return Created(loaction, new Response<ReportResponse>(response));
        }

        [HttpGet(ApiRoutes.Report.Get)]
        public async Task<IActionResult> Get([FromRoute] Guid id)
        {
            var report = await _reportService.GetByIdAsync(id);
            if(report == null)
            {
                return NotFound();
            }

            var response = report.ToReponse();
            return Ok(new Response<ReportResponse>(response));
        }

        [HttpGet(ApiRoutes.Report.GetAll)]
        public async Task<IActionResult> GetAll([FromQuery] GetAllReportsFilter filter, [FromQuery] PaginationFilter pagination)
        {
            var posts = await _reportService.GetAllAsync(filter, pagination);
            var responses = posts.Select(x => x.ToReponse()).ToList();
            if (pagination == null || pagination.PageNumber < 1 || pagination.PageSize < 1)
            {
                return Ok(new PagedResponse<ReportResponse>(responses));
            }
            var paginationResponse = PaginationHelpers.CreatePaginatedResponse(_uriService, ApiRoutes.Post.GetAll, pagination, responses);
            return Ok(paginationResponse);
        }

        [HttpPatch(ApiRoutes.Report.Update)]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateReportRequest request)
        {
            var existingReport = await _reportService.GetByIdAsync(id);
            if (existingReport == null)
            {
                return NotFound();
            }

            if (existingReport.CreatorId != HttpContext.GetUserId() && !HttpContext.HasRole("Admin") && !HttpContext.HasRole("ContentAdmin"))
            {
                return Unauthorized(StaticErrorMessages.PermissionDenied);
            }

            existingReport.Status = request.Status;

            var result = await _reportService.UpdateAsync(existingReport);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            var response = result.Data.ToReponse();
            return Ok(new Response<ReportResponse>(response));
        }
    }
}
