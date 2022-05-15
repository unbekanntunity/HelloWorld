using HelloWorldAPI.Contracts;
using HelloWorldAPI.Domain;
using HelloWorldAPI.Domain.Database;
using HelloWorldAPI.Domain.Filters;
using HelloWorldAPI.Options;
using HelloWorldAPI.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HelloWorldAPI.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly JwtSettings _jwtSettings;
        private readonly TokenValidationParameters _tokenValidationParameters;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly INonQueryRepository<RefreshToken> _nonQueryRepository;

        public IdentityService(UserManager<User> userManager, JwtSettings jwtSettings, TokenValidationParameters tokenValidationParameters, RoleManager<IdentityRole> roleManager, IRefreshTokenRepository refreshTokenRepository, INonQueryRepository<RefreshToken> nonQueryRepository)
        {
            _userManager = userManager;
            _jwtSettings = jwtSettings;
            _tokenValidationParameters = tokenValidationParameters;
            _roleManager = roleManager;
            _refreshTokenRepository = refreshTokenRepository;
            _nonQueryRepository = nonQueryRepository;
        }

        public async Task<AuthenticationResult> LoginAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return new AuthenticationResult
                {
                    Errors = new[] { "User does not exists" }
                };
            }

            var userHasValidPassword = await _userManager.CheckPasswordAsync(user, password);
            if (!userHasValidPassword)
            {
                return new AuthenticationResult
                {
                    Errors = new[] { "User/Password is invalid" }
                };
            }

            return await GenerateTokenAsync(user);
        }

        public async Task<AuthenticationResult> RefreshTokenAsync(string token, string refreshToken)
        {
            var validatedToken = GetPrincipal(token);

            if (validatedToken == null)
            {
                return new AuthenticationResult
                {
                    Errors = new[] { "Invalid Token" }
                };
            }

            var expiryDateUnix = long.Parse(validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Exp).Value);

            var expiryDateTimeUtc = new DateTime(1970, 1, 1, 1, 0, 0, DateTimeKind.Utc)
                .AddSeconds(expiryDateUnix);

            if (expiryDateTimeUtc > DateTime.UtcNow)
            {
                return new AuthenticationResult
                {
                    Errors = new[] { "This token hasnt expired yet" }
                };
            }

            var jti = validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Jti).Value;

            var storedRefreshToken = await _refreshTokenRepository.GetByPredicateAsync(x => x.Token == refreshToken);
            if (storedRefreshToken == null)
            {
                return new AuthenticationResult
                {
                    Errors = new[] { "This refresh token does not exists" }
                };
            }

            if (DateTime.UtcNow > storedRefreshToken.ExpiryDate)
            {
                return new AuthenticationResult
                {
                    Errors = new[] { "This refresh token has expired" }
                };
            }

            if (storedRefreshToken.Invalidated)
            {
                return new AuthenticationResult
                {
                    Errors = new[] { "This refresh token has been invalidated" }
                };
            }

            if (storedRefreshToken.JwtId == jti)
            {
                return new AuthenticationResult
                {
                    Errors = new[] { "This refresh token does not match with the jwt" }
                };
            }

            storedRefreshToken.Used = true;
            await _nonQueryRepository.UpdateAsync(storedRefreshToken);

            var user = await _userManager.FindByIdAsync(validatedToken.Claims.Single(x => x.Type == "id").Value);
            return await GenerateTokenAsync(user);
        }

        private ClaimsPrincipal GetPrincipal(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                var principle = tokenHandler.ValidateToken(token, _tokenValidationParameters, out var validatedToken);
                if (!IsJwtWithValidSecurityAlgorithm(validatedToken))
                {
                    return null;
                }
                return principle;
            }
            catch
            {
                return null;
            }
        }

        private static bool IsJwtWithValidSecurityAlgorithm(SecurityToken validatedToken)
        {
            return validatedToken is JwtSecurityToken jwtSecurityToken &&
                jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);
        }

        public async Task<AuthenticationResult> RegisterAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user != null)
            {
                return new AuthenticationResult
                {
                    Errors = new[] { "User with this email already exists" }
                };
            }

            var new_user = new User
            {
                Email = email,
                CreatedAt = DateTime.UtcNow
            };

            var createdUser = await _userManager.CreateAsync(new_user, password);

            if (!createdUser.Succeeded)
            {
                return new AuthenticationResult
                {
                    Errors = createdUser.Errors.Select(x => x.Description)
                };
            }

            return await GenerateTokenAsync(new_user);
        }

        private async Task<AuthenticationResult> GenerateTokenAsync(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);

            var claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Name, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("id", user.Id),
            };

            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var userClaims = await _userManager.GetClaimsAsync(user);
            claims.AddRange(userClaims);

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.Add(_jwtSettings.TokenLifeTime),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var refreshToken = new RefreshToken()
            {
                JwtId = token.Id,
                UserId = user.Id,
                CreationDate = DateTime.Now,
                ExpiryDate = DateTime.Now.AddDays(_jwtSettings.RefreshTokenLifeTime),
            };

            var succeeded = await _nonQueryRepository.CreateAsync(refreshToken);
            return succeeded ? new AuthenticationResult
            {
                Success = true,
                Token = tokenHandler.WriteToken(token),
                RefreshToken = refreshToken.Token
            } :
            new AuthenticationResult
            {
                Errors = new string[] { "Refreshtoken could not be created" }
            };
        }

        public async Task<Result> DeleteUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return new Result
                {
                    Errors = new string[] { StaticErrorMessages<User>.NotFound }
                };
            }

            var userResult = await _userManager.DeleteAsync(user);

            var tokens = await _refreshTokenRepository.GetAllByPredicateAsync(x => x.UserId == userId);
            await _nonQueryRepository.DeleteRangeAsync(tokens.ToArray());

            return new Result
            {
                Success = userResult.Succeeded,
                Errors = userResult.Errors.Select(x => x.Description)
            };
        }

        public async Task<Result<User>> CreateUserAsync(User newUser, string[] roles, string newPassword)
        {
            newUser.CreatedAt = DateTime.UtcNow;
            newUser.UpdatedAt = DateTime.UtcNow;

            var result = await _userManager.CreateAsync(newUser, newPassword);
            var user = await GetUserByIdAsync(newUser.Id);

            roles = roles.Where(x => _roleManager.Roles.Select(x => x.Name).Contains(x)).ToArray();
            await _userManager.AddToRolesAsync(user, roles);

            return new Result<User>
            {
                Success = result.Succeeded,
                Data = result.Succeeded ? newUser : null,
                Errors = result.Errors.Select(x => x.Description),
            };
        }

        public async Task<User> GetUserByIdAsync(string userId) => await _userManager.FindByIdAsync(userId);
        public async Task<List<User>> GetUsersAsync(GetAllUserFilter filter = null, PaginationFilter pagination = null)
        {
            var queryable = _userManager.Users
                .Include(x => x.Articles)
                .Include(x => x.Comments)
                .Include(x => x.Posts)
                .Include(x => x.Projects)
                .Include(x => x.Discussions)
                .Include(x => x.ArticlesLiked)
                .Include(x => x.CommentsLiked)
                .Include(x => x.PostsLiked)
                .Include(x => x.ProjectsLiked)
                .Include(x => x.Tags)
                .Include(x => x.Replies)
                .AsQueryable();

            if (pagination == null)
            {
                return await queryable.ToListAsync();
            }
            if (filter != null)
            {
                queryable = AddFiltersOnQuery(filter, queryable);
            }

            var skip = (pagination.PageNumber - 1) * pagination.PageSize;
            return await queryable.Skip(skip).Take(pagination.PageSize).ToListAsync();
        }

        public async Task<Result<User>> UpdateUserAsync(User user)
        {
            user.UpdatedAt = DateTime.UtcNow;
            var updated = await _userManager.UpdateAsync(user); 

            return new Result<User>
            {
                Success = updated.Succeeded,
                Data = updated.Succeeded ? user : null,
                Errors = updated.Errors.Select(x => x.Description),
            };
        }

        public async Task<Result<string>> UpdateLoginAsync(string userId, string newEmail, string oldPassword, string newPassword)
        {
            var user = await GetUserByIdAsync(userId);
            var samePassword = _userManager.PasswordHasher.VerifyHashedPassword(user, user.PasswordHash, oldPassword);

            if (samePassword != PasswordVerificationResult.Success)
            {
                return new Result<string>
                {
                    Errors = new string[] { "Old password does not match with the provided one" }
                };
            }

            user.Email = newEmail;
            var result = await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);
            if (!result.Succeeded)
            {
                return new Result<string>
                {
                    Errors = result.Errors.Select(x => x.Description)
                };
            }

            var updated = await UpdateUserAsync(user);
            return new Result<string>
            {
                Success = updated.Success,
                Data = updated.Success ? newPassword : string.Empty,
                Errors = updated.Errors
            };
        }

        public async Task<bool> OwnUser(string ownId, string targetId)
        {
            var ownUser = await _userManager.FindByIdAsync(ownId);
            var targetUser = await _userManager.FindByIdAsync(ownId);
            if (ownUser == null || targetUser == null)
            {
                return false;
            }

            return ownUser.Id == targetUser.Id;
        }

        public async Task<List<string>> GetAllRolesOfUserAsync(User user) => (await _userManager.GetRolesAsync(user)).ToList();

        public async Task<Result<User>> AddProjectToMemberAsync(User user, Project project, ProjectRole role)
        {
            if (user.Projects == null)
            {
                user.Projects = new List<Project> { project };
            }
            else
            {
                user.ProjectsJoined.Add(project);
            }
            var updateResult = await _userManager.UpdateAsync(user);
            return new Result<User>
            {
                Success = updateResult.Succeeded,
                Data = updateResult.Succeeded ? user : null,
                Errors = updateResult.Errors.Select(x => x.Description)
            };
        }

        public async Task<Result<User>> RemoveProjectFromMemberAsync(User user, Project project, ProjectRole role)
        {
            if (role == ProjectRole.Creator)
            {
                user.Projects.Remove(project);
            }
            else if (role == ProjectRole.Member)
            {
                user.ProjectsJoined.Remove(project);
            }

            var updateResult = await _userManager.UpdateAsync(user);
            return new Result<User>
            {
                Success = updateResult.Succeeded,
                Data = updateResult.Succeeded ? user : null,
                Errors = updateResult.Errors.Select(x => x.Description)
            };
        }

        private static IQueryable<User> AddFiltersOnQuery(GetAllUserFilter filter, IQueryable<User> queryable)
        {
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
