using API.Data;
using API.Domain.Database;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories
{
    public class ReportRepository : IReportRepository
    {
        private readonly DataContext _dataContext;

        public ReportRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<List<Report>> GetAllAsync() => await _dataContext.Reports.ToListAsync();

        public async Task<Report?> GetByIdAsync(Guid id) => await _dataContext.Reports.FirstOrDefaultAsync(x => x.Id == id);
    }
}
