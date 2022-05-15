using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Testing2.Data;
using Testing2.Domain;
using Testing2.MockingControlers;
using Testing2.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddScoped<ITestService, TestService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
}



app.Run();
