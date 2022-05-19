using HelloWorldAPI.Contracts.HealthChecks;
using HelloWorldAPI.Installers;
using HelloWorldAPI.Options;
using HelloWorldAPI.Services;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Newtonsoft.Json;

namespace HelloWorldAPI
{
    public class Program
    {
        public static async Task Main(string[] args)
        {

            var builder = WebApplication.CreateBuilder(args);
            builder.Services.InstallServicesInAssembly(builder.Configuration);

            var app = builder.Build();


            using (var scope = app.Services.CreateScope())
            {
                var seedService = scope.ServiceProvider.GetRequiredService<ISeedService>();
                await seedService.SeedDatabase();
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseHsts();
            }

            var swaggerOptions = new SwaggerOptions();
            app.Configuration.GetSection(nameof(SwaggerOptions)).Bind(swaggerOptions);

            var redisOptions = new RedisCacheSettings();
            app.Configuration.GetSection(nameof(RedisCacheSettings)).Bind(redisOptions);

            if (redisOptions.Enabled)
            {
                app.UseHealthChecks("/health", new HealthCheckOptions
                {
                    ResponseWriter = async (context, report) =>
                    {
                        context.Response.ContentType = "application/json";

                        var response = new HealthCheckResponse
                        {
                            Status = report.Status.ToString(),
                            Checks = report.Entries.Select(x => new HealthCheck
                            {
                                Component = x.Key,
                                Status = x.Value.Status.ToString(),
                                Description = x.Value.Description
                            }),
                            Duration = report.TotalDuration
                        };

                        await context.Response.WriteAsync(JsonConvert.SerializeObject(response));
                    }
                });
            }

            app.UseSwagger(option => { option.RouteTemplate = swaggerOptions.JsonRoute; });
            app.UseSwaggerUI(option => option.SwaggerEndpoint(swaggerOptions.UIEndPoint, swaggerOptions.Description));

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });

            app.MapRazorPages();
            app.Run();
        }
    }
}