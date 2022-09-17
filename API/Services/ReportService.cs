using API.Contracts;
using API.Domain;
using API.Domain.Database;
using API.Domain.Filters;
using API.Extensions;
using API.Repositories;

namespace API.Services
{
    public class ReportService : IReportService
    {
        private readonly IReportRepository _reportRepository;
        private readonly INonQueryRepository<Report> _nonQueryRepository;

        public ReportService(IReportRepository reportRepository, INonQueryRepository<Report> nonQueryRepository)
        {
            _reportRepository = reportRepository;
            _nonQueryRepository = nonQueryRepository;
        }

        public async Task<Result<Report>> CreateAsync(Report report)
        {
            report.CreatedAt = DateTime.Now;
            report.UpdatedAt = DateTime.Now;
            report.Status = "Open"; 

            var result = await _nonQueryRepository.CreateAsync(report);

            return new Result<Report>
            {   
                Success = result,
                Data = result ? report : null,
                Errors = result ? Array.Empty<string>() : new string[] { StaticErrorMessages<Report>.CreateOperationFailed },
            };
        }

        public async Task<List<Report>> GetAllAsync(GetAllReportsFilter filter = null, PaginationFilter pagination = null)
        {
            var queryable = (await _reportRepository.GetAllAsync()).AsQueryable();
            if (pagination == null)
            {
                return await queryable.ToListAsyncSafe();
            }
            if (filter != null)
            {
                queryable = AddFiltersOnQuery(filter, queryable);
            }

            var skip = (pagination.PageNumber - 1) * pagination.PageSize;

            return await (queryable.Skip(skip).Take(pagination.PageSize)).ToListAsyncSafe();
        }

        public async Task<Report?> GetByIdAsync(Guid reportId)
        {
            return await _reportRepository.GetByIdAsync(reportId);
        }

        public async Task<Result<Report>> UpdateAsync(Report report)
        {
            report.UpdatedAt = DateTime.Now;

            var success = await _nonQueryRepository.UpdateAsync(report);
            return new Result<Report>
            {
                Success = success,
                Data = success ? report : null,
                Errors = success ? Array.Empty<string>() : new string[] { StaticErrorMessages<Report>.UpdateOperationFailed }
            };
        }

        private static IQueryable<Report> AddFiltersOnQuery(GetAllReportsFilter filter, IQueryable<Report> queryable)
        {
            if (!string.IsNullOrEmpty(filter.CreatorId))
            {
                queryable = queryable.Where(x => x.CreatorId == filter.CreatorId);
            }
            if (!string.IsNullOrEmpty(filter.ModId))
            {
                queryable = queryable.Where(x => x.ModId == filter.ModId);
            }
            if (!string.IsNullOrEmpty(filter.Status))
            {
                queryable = queryable.Where(x => x.Status == filter.Status);
            }
            if (filter.CreatedAt != null)
            {
                queryable = queryable.Where(x => x.CreatedAt == filter.CreatedAt);
            }
            if (filter.UpdatedAt != null)
            {
                queryable = queryable.Where(x => x.UpdatedAt == filter.UpdatedAt);
            }
         

            return queryable;
        }
    }
}
