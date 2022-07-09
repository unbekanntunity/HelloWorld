using API.Data;

namespace API.Installers
{
    public class HealthChecksInstaller : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            //services.AddHealthChecks()
            //    .AddDbContextCheck<DataContext>();
        }
    }
}
