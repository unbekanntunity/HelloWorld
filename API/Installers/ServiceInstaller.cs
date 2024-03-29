﻿using API.Repositories;
using API.Services;

namespace API.Installers
{
    public class ServiceInstaller : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IIdentityService, IdentityService>();

            services.AddSingleton<IUriService>(provider =>
            {
                var accessor = provider.GetRequiredService<IHttpContextAccessor>();
                var request = accessor.HttpContext.Request;
                var absoluteUri = string.Concat(request.Scheme, "://", request.Host.ToUriComponent(), "/");
                return new UriService(absoluteUri);
            });

            services.AddScoped<IRefreshTokenRepository, RefreshtokenRepository>();
            services.AddScoped<IDiscussionRepository, DiscussionRepository>();
            services.AddScoped<ITagRepository, TagRepository>();
            services.AddScoped<IArticleRepository, ArticleRepository>();
            services.AddScoped<IProjectRepository, ProjectRepository>();
            services.AddScoped<IPostRepository, PostRepository>();
            services.AddScoped<ICommentRepository, CommentRepository>();
            services.AddScoped<IReplyRepository, ReplyRepository>();
            services.AddScoped<IReportRepository, ReportRepository>();
            services.AddScoped(typeof(INonQueryRepository<>), typeof(NonQueryRepository<>));

            services.AddScoped<ISeedService, SeedService>();
            services.AddScoped<IDiscussionService, DiscussionService>();
            services.AddScoped<ITagService, TagService>();
            services.AddScoped<IArticleService, ArticleService>();
            services.AddScoped<IProjectService, ProjectService>();
            services.AddScoped<IPostService, PostService>();
            services.AddScoped<ICommentService, CommentService>();
            services.AddScoped<IReplyService, ReplyService>();
            services.AddScoped<IReportService, ReportService>();
            services.AddScoped(typeof(IRateableService<>), typeof(RateableService<>));
            services.AddScoped(typeof(ISaveService<>), typeof(SaveService<>));

            services.AddSingleton<IFileManager>(provider =>
            {
                var accessor = provider.GetRequiredService<IHttpContextAccessor>();
                var request = accessor.HttpContext.Request;
                var absoluteUri = string.Concat(request.Scheme, "://", request.Host.ToUriComponent(), "/");
                return new FileManager("wwwroot", absoluteUri);
            });
        }
    }
}
