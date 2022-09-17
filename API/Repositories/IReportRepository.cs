using API.Domain.Database;

namespace API.Repositories
{
    public interface IReportRepository
    {
        Task<Report?> GetByIdAsync(Guid id);
        Task<List<Report>> GetAllAsync();
    }
}
