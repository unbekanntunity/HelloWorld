using FluentAssertions;
using HelloWorldAPI.Contracts.V1;
using HelloWorldAPI.Contracts.V1.Requests;
using HelloWorldAPI.Contracts.V1.Responses;
using HelloWorldAPI.Domain.Filters;
using HelloWorldAPI.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace HelloWorld.IntegrationTests
{
    public class UserControllerTests : IntegrationTests
    {
        // Add another check where I get the updated user from the db and not from the response

        [Fact]
        public async Task Create_ReturnsForbidden_WhenNoAdminAccount()
        {
            //Arrange
            await AuthenticateAsync();

            //Act
            var response = await TestClient.PostAsJsonAsync(ApiRoutes.Identity.Create, new CreateUserRequest
            {
                Description = "Test",
                Email = "Valid@gmail.com",
                Password = "Valid1234!",
                UserName = "Valid"
            });

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Create_ReturnsCreatedUser_WhenAdminAccount()
        {
            //Arrange
            var userName = Guid.NewGuid().ToString();
            var description = "Integration test are nice";
            var email = "IntegrationTest@exmaple.com";

            await AuthenticateAsAdminAsync();

            //Act
            var response = await TestClient.PostAsJsonAsync(ApiRoutes.Identity.Create, new CreateUserRequest
            {
                UserName = userName,
                Description = description,
                Email = email,
                Password = "SomePass1234!",
                RoleNames = new List<string>()
            });

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            var returnedUser = await response.Content.ReadAsAsync<Response<UserResponse>>();
            returnedUser.Data.UserName.Should().Be(userName);
            returnedUser.Data.Description.Should().Be(description);
            returnedUser.Data.Email.Should().Be(email);
            returnedUser.Data.UpdatedAt.Day.Should().Be(DateTime.UtcNow.Day);
            returnedUser.Data.CreatedAt.Day.Should().Be(DateTime.UtcNow.Day);

            var doubleCheck = await TestClient.GetAsync(ApiRoutes.Identity.Get.Replace("{id}", returnedUser.Data.Id));
            doubleCheck.StatusCode.Should().Be(HttpStatusCode.OK);

            var doubleCheckUser = await doubleCheck.Content.ReadAsAsync<Response<UserResponse>>();
            doubleCheckUser.Data.UpdatedAt.Day.Should().Be(DateTime.UtcNow.Day);
            doubleCheckUser.Data.CreatedAt.Day.Should().Be(DateTime.UtcNow.Day);
        }

        [Fact]
        public async Task Delete_ReturnsEmptyResponse_WhenAdminAccount()
        {
            //Arrange
            var token = await AuthenticateAsync();
            await AuthenticateAsAdminAsync();

            //Act
            var response = await TestClient.DeleteAsync(ApiRoutes.Identity.Delete.Replace("{id}", GetUserId(token)));
       
            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);     
            
            var doubleCheck = await TestClient.GetAsync(ApiRoutes.Identity.Get.Replace("{id}", GetUserId(token)));
            doubleCheck.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
        
        [Fact]
        public async Task Delete_ReturnsForbidden_WhenNoAdminAccount()
        {
            //Arrange
            var token = await AuthenticateAsync();

            //Act
            var response = await TestClient.DeleteAsync(ApiRoutes.Identity.Delete.Replace("{id}", GetUserId(token)));

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task DeleteOwn_ReturnsEmptyResponse_WhenOwn()
        {
            //Arrange
            var token = await AuthenticateAsync();

            //Act
            var response = await TestClient.DeleteAsync(ApiRoutes.Identity.DeleteOwn);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);

            var doubleCheck = await TestClient.GetAsync(ApiRoutes.Identity.Get.Replace("{id}", GetUserId(token)));
            doubleCheck.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task DeleteOwn_ReturnsUnAuthorized_WhenNoAccount()
        {
            var createdUser = await CreateUserAsync(new CreateUserRequest
            {
                Description = "New",
                Email = "New@gmail.com",
                Password = "New1234!",
                RoleNames = new List<string>(),
                UserName = "New"
            });

            //Act
            var response = await TestClient.DeleteAsync(ApiRoutes.Identity.DeleteOwn);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

            await AuthenticateAsync();

            var doubleCheck = await TestClient.GetAsync(ApiRoutes.Identity.Get.Replace("{id}", createdUser.Id.ToString()));
            doubleCheck.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Get_ReturnsUser_WhenUserExists()
        {
            //Arrange
            var userName = Guid.NewGuid().ToString();
            var description = "Integration test are nice";
            var email = "IntegrationTest@exmaple.com";

            var createdUser = await CreateUserAsync(new CreateUserRequest
            {
                UserName = userName,
                Description = description,
                Email = email,
                Password = "SomePass1234!",
                RoleNames = new List<string>()
            });
            await AuthenticateAsync();

            //Act 
            var response = await TestClient.GetAsync(ApiRoutes.Identity.Get.Replace("{id}", createdUser.Id));

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var returnedUser = await response.Content.ReadAsAsync<Response<UserResponse>>();
            returnedUser.Data.UserName.Should().Be(userName);
            returnedUser.Data.Description.Should().Be(description);
            returnedUser.Data.Email.Should().Be(email);
        }

        [Fact]
        public async Task Get_ReturnsNotfound_WhenUserNotExists()
        {
            //Arrange
            await AuthenticateAsync();

            //Act 
            var response = await TestClient.GetAsync(ApiRoutes.Identity.Get.Replace("{id}", "RandomString"));

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetAll_ReturnsOnlySeededData_WhenFreshDatabase()
        {
            //Arrange
            await AuthenticateAsync();

            //Act
            var response = await TestClient.GetAsync(ApiRoutes.Identity.GetAll);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var returnedUsers = await response.Content.ReadAsAsync<Response<List<UserResponse>>>();
            returnedUsers.Data.Should().HaveCount(5);
        }

        [Fact]
        public async Task GetAll_ReturnsAdminAccount_WhenFreshDatabaseAndUserNameFilter()
        {
            //Arrange
            var userName = "Admin";
            await AuthenticateAsync();

            var filter = new GetAllUserFilter
            {
                UserName = userName,
            };

            //Act
            var response = await TestClient.GetAsync(ApiRoutes.Identity.GetAll + filter.ToQueryString());

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var returnedUsers = await response.Content.ReadAsAsync<PagedResponse<PartialUserResponse>>();
            returnedUsers.Data.Should().HaveCount(1);

            returnedUsers.Data.Select(x => x.UserName).First().Should().Contain(userName);
        }

        [Fact]
        public async Task GetAll_ReturnsCorrectPagination_WhenHaveData()
        {
            //Arrange

            var pageNumber = 2;
            var pageSize = 1;
            var paginationFilter = new PaginationFilter
            {
                PageNumber = 2,
                PageSize = 1
            };

            await AuthenticateAsync();

            //Act
            var response = await TestClient.GetAsync(ApiRoutes.Identity.GetAll + paginationFilter.ToQueryString());

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var returnedComments = await response.Content.ReadAsAsync<PagedResponse<PartialUserResponse>>();
            returnedComments.NextPage.Should().Be(GetAllUriNext(ApiRoutes.Identity.GetAll, pageNumber, pageSize));
            returnedComments.PreviousPage.Should().Be(GetAllUriLast(ApiRoutes.Identity.GetAll, pageNumber, pageSize));
            returnedComments.PageNumber.Should().Be(pageNumber);
        }

        [Fact]
        public async Task GetAll_ReturnsSixUsers_WhenOneUserAdded()
        {
            //Arrange
            await CreateUserAsync(new CreateUserRequest
            {
                Description = "Hello",
                Email = "Helo@gmail.com",
                Password = "Hello1234!",
                RoleNames = new List<string>(),
                UserName = "Hello"
            });

            await AuthenticateAsync();

            //Act
            var response = await TestClient.GetAsync(ApiRoutes.Identity.GetAll);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            (await response.Content.ReadAsAsync<Response<List<UserResponse>>>()).Data.Should().HaveCount(6);
        }

        [Fact]
        public async Task GetAll_ReturnsUnAuthorized_WhenNoAccount()
        {
            //Act
            var response = await TestClient.GetAsync(ApiRoutes.Identity.GetAll);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Update_ReturnUpdatedUser_WhenAccount()
        {
            //Arrange
            var email = "Integration@gmail.com";
            var userName = Guid.NewGuid().ToString();
            var newDescription = "Integration tests are tough";

            var token = await RegisterUserAsync(new UserRegistrationRequest
            {
                UserName = userName,
                Email = email,
                Password = "SomePass1234!",
            });

            AuthenticateWithToken(token);

            //Act
            var response = await TestClient.PatchAsync(ApiRoutes.Identity.Update, JsonContent.Create(new UpdateUserRequest
            {
                UserName = userName,
                Description = newDescription,
                TagNames = new List<string>()
            }));

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var returnedUser = await response.Content.ReadAsAsync<Response<UserResponse>>();
            returnedUser.Data.UserName.Should().Be(userName);
            returnedUser.Data.Description.Should().Be(newDescription);
            returnedUser.Data.Tags.Should().BeEmpty();
            returnedUser.Data.UpdatedAt.Day.Should().Be(DateTime.Today.Day);


            var doubleCheck = await TestClient.GetAsync(ApiRoutes.Identity.Get.Replace("{id}", returnedUser.Data.Id));
            doubleCheck.StatusCode.Should().Be(HttpStatusCode.OK);

            var doubleCheckUser = await doubleCheck.Content.ReadAsAsync<Response<UserResponse>>();
            doubleCheckUser.Data.Description.Should().Be(newDescription);
        }

        [Fact]
        public async Task UpdateLogin_ReturnsUpdatedPassword_WhenOwnAndCorrectPassword()
        {
            //Arrange
            var email = "Integration@gmail.com";
            var userName = Guid.NewGuid().ToString();

            var newPassword = "SomeNewPass1234!";
            var password = "SomePass1234!";

            var token = await RegisterUserAsync(new UserRegistrationRequest
            {
                UserName = userName,
                Email = email,
                Password = password,
            });

            AuthenticateWithToken(token);

            //Act
            var response = await TestClient.PatchAsync(ApiRoutes.Identity.UpdateLogin.Replace("{id}", GetUserId(token)), JsonContent.Create(new UpdateLoginRequest
            {
                Email = email,
                OldPassword = password,
                NewPassword = newPassword
            }));

            var e = await response.Content.ReadAsStringAsync();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var returnedPassword = await response.Content.ReadAsAsync<Response<string>>();
            returnedPassword.Data.Should().Be(newPassword);

            var loginResponse = await TestClient.PostAsJsonAsync(ApiRoutes.Identity.Login, new UserLoginRequest
            {
                Email = email,
                Password = newPassword
            });

            loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task UpdateLogin_ReturnsBadRequest_WhenOwnAndInCorrectPassword()
        {
            //Arrange
            var email = "Integration@gmail.com";
            var userName = Guid.NewGuid().ToString();

            var newPassword = "SomeNewPass1234!";
            var password = "SomePass1234!";

            var token = await RegisterUserAsync(new UserRegistrationRequest
            {
                UserName = userName,
                Email = email,
                Password = password,
            });

            AuthenticateWithToken(token);

            //Act
            var response = await TestClient.PatchAsync(ApiRoutes.Identity.UpdateLogin.Replace("{id}", GetUserId(token)), JsonContent.Create(new UpdateLoginRequest
            {
                Email = email,
                OldPassword = "WrongPassword",
                NewPassword = newPassword
            }));

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var loginResponse = await TestClient.PostAsJsonAsync(ApiRoutes.Identity.Login, new UserLoginRequest
            {
                Email = email,
                Password = newPassword
            });
            loginResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task UpdateLogin_ReturnsUpdatedPassword_WhenAdminAndCorrectPassword()
        {
            //Arrange
            var email = "Integration@gmail.com";
            var userName = Guid.NewGuid().ToString();

            var newPassword = "SomeNewPass1234!";
            var password = "SomePass1234!";

            var token = await RegisterUserAsync(new UserRegistrationRequest
            {
                UserName = userName,
                Email = email,
                Password = password,
            });

            AuthenticateWithToken(token);

            await AuthenticateAsAdminAsync();

            //Act
            var response = await TestClient.PatchAsync(ApiRoutes.Identity.UpdateLogin.Replace("{id}", GetUserId(token)), JsonContent.Create(new UpdateLoginRequest
            {
                Email = email,
                OldPassword = password,
                NewPassword = newPassword
            }));

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK); 
            var returnedPassword = await response.Content.ReadAsAsync<Response<string>>();
            returnedPassword.Data.Should().Be(newPassword);

            var loginResponse = await TestClient.PostAsJsonAsync(ApiRoutes.Identity.Login, new UserLoginRequest
            {
                Email = email,
                Password = newPassword
            });
            loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}
