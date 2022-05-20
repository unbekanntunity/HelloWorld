using HelloWorldAPI.Data;
using HelloWorldAPI.Domain.Database;
using HelloWorldAPI.Extensions;
using Microsoft.AspNetCore.Identity;

namespace HelloWorldAPI.Services
{
    public class SeedService : ISeedService
    {
        private readonly DataContext _dataContext;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<User> _userManager;

        private readonly bool reset = false;

        public SeedService(RoleManager<IdentityRole> roleManager, UserManager<User> userManager, DataContext dataContext)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _dataContext = dataContext;
        }

        public async Task SeedDatabase()
        {
            if(reset)
            {
                await _dataContext.Database.EnsureDeletedAsync();
                return;
            }

            if (!await _roleManager.RoleExistsAsync("ContentAdmin"))
            {
                var contentAdminRole = new IdentityRole
                {
                    Name = "ContentAdmin",
                };
                await _roleManager.CreateAsync(contentAdminRole);
            }
            if (!await _roleManager.RoleExistsAsync("UserAdmin"))
            {
                var userAdminRole = new IdentityRole
                {
                    Name = "UserAdmin",
                };
                await _roleManager.CreateAsync(userAdminRole);
            }
            if (!await _roleManager.RoleExistsAsync("RootAdmin"))
            {
                var userAdminRole = new IdentityRole
                {
                    Name = "RootAdmin",
                };
                await _roleManager.CreateAsync(userAdminRole);
            }

            if (await _userManager.FindByNameAsync("Admin") == null)
            {
                var user = new User { UserName = "Admin", Email = "Admin@gmail.com" };
                await _userManager.CreateAsync(user, "Admin1234!");
                await _userManager.AddToRoleAsync(user, "RootAdmin");
                await _userManager.AddToRoleAsync(user, "ContentAdmin");
                await _userManager.AddToRoleAsync(user, "UserAdmin");
            }
            if (await _userManager.FindByNameAsync("ContentAdmin") == null)
            {
                var user = new User { UserName = "ContentAdmin", Email = "ConAdmin@gmail.com" };
                await _userManager.CreateAsync(user, "Admin1234!");
                await _userManager.AddToRoleAsync(user, "ContentAdmin");
            }
            if (await _userManager.FindByNameAsync("UserAdmin") == null)
            {
                var user = new User { UserName = "UserAdmin", Email = "UserAdmin@gmail.com" };
                await _userManager.CreateAsync(user, "Admin1234!");
                await _userManager.AddToRoleAsync(user, "UserAdmin");
            }
            if (await _userManager.FindByNameAsync("NormalUser") == null)
            {
                var user = new User { UserName = "NormalUser", Email = "User@gmail.com" };
                await _userManager.CreateAsync(user, "User1234!");
            }
            if (await _userManager.FindByNameAsync("NormalUser2") == null)
            {
                var user = new User { UserName = "NormalUser2", Email = "User2@gmail.com" };
                await _userManager.CreateAsync(user, "User1234!");
            }
        }
    }
}
