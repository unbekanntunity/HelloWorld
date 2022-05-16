using HelloWorldAPI.Domain;
using HelloWorldAPI.Domain.Database;
using HelloWorldAPI.Domain.Filters;

namespace HelloWorldAPI.Services
{

    public interface IIdentityService
    {
        Task<AuthenticationResult> RegisterAsync(string userName, string email, string password);
        Task<AuthenticationResult> LoginAsync(string email, string password);
        Task<AuthenticationResult> RefreshTokenAsync(string token, string refreshToken);

        Task<Result<User>> CreateUserAsync(User new_user, string[] roles, string newPassword);
        Task<Result> DeleteUserAsync(string userId);
        Task<User> GetUserByIdAsync(string userId);
        Task<List<User>> GetUsersAsync(GetAllUserFilter filter = null, PaginationFilter pagination = null);
        Task<Result<User>> UpdateUserAsync(User user, IEnumerable<string> TagNames);
        Task<Result<string>> UpdateLoginAsync(string userId, string newEmail, string oldPassword, string newPassword);

        Task<bool> OwnUser(string ownId, string targetId);

        Task<List<string>> GetAllRolesOfUserAsync(User user);

        Task<Result<User>> AddProjectToMemberAsync(User user, Project project);
        Task<Result<User>> RemoveProjectFromMemberAsync(User user, Project project);
    }
}
