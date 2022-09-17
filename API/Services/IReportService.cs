using API.Domain;
using API.Domain.Database;
using API.Domain.Filters;

namespace API.Services
{
    public interface IReportService
    {
        Task<Result<Report>> CreateAsync(Report report);
        Task<Result<Report>> UpdateAsync(Report report);
        Task<Report?> GetByIdAsync(Guid articleId);
        Task<List<Report>> GetAllAsync(GetAllReportsFilter filter = null, PaginationFilter pagination = null);
    }
}
