using FluentAssertions;
using FluentValidation.Results;
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
    public class PostControllerTests : IntegrationTests
    {
        [Fact]
        public async Task Create_ReturnsUnAuthorized_WhenNoAccount()
        {
            var response = await TestClient.PostAsync(ApiRoutes.Post.Create, JsonContent.Create(new CreatePostRequest
            {
                Title = "Test",
                Content = "Test",
                TagNames = new List<string>()
            }));

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Create_ReturnsCreated_WhenAccount()
        {
            //Arrange
            var title = "Test post";
            var content = "Test";
            var tagNames = new List<string> { "New Tag" };

            await AuthenticateAsync();

            //Act
            var response = await TestClient.PostAsync(ApiRoutes.Post.Create, JsonContent.Create(new CreatePostRequest
            {
                Title = title,
                Content = content,
                TagNames = tagNames
            }));

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            var returnedPost = await response.Content.ReadAsAsync<Response<PostResponse>>();
            returnedPost.Data.Title.Should().Be(title);
            returnedPost.Data.Content.Should().Be(content);
            returnedPost.Data.Tags.Select(x => x.Name).Should().BeEquivalentTo(tagNames);
            returnedPost.Data.UpdatedAt.Day.Should().Be(DateTime.UtcNow.Day);
            returnedPost.Data.CreatedAt.Day.Should().Be(DateTime.UtcNow.Day);

            var doubleCheck = await TestClient.GetAsync(ApiRoutes.Post.Get.Replace("{id}", returnedPost.Data.Id.ToString()));
            doubleCheck.StatusCode.Should().Be(HttpStatusCode.OK);

            var doubleCheckPost = await doubleCheck.Content.ReadAsAsync<Response<PostResponse>>();
            doubleCheckPost.Data.Title.Should().Be(title);
            doubleCheckPost.Data.Content.Should().Be(content);
            doubleCheckPost.Data.Tags.Select(x => x.Name).Should().BeEquivalentTo(tagNames);
            doubleCheckPost.Data.UpdatedAt.Day.Should().Be(DateTime.UtcNow.Day);
            doubleCheckPost.Data.CreatedAt.Day.Should().Be(DateTime.UtcNow.Day);
        }

        [Fact]
        public async Task Delete_ReturnsNoContent_WhenOwnAndPostExists()
        {
            //Arrange
            var createdPost = await CreatePostAsync(new CreatePostRequest
            {
                Title = "New Post",
                Content = "New Content",
                TagNames = new List<string>(),
            });

            await AuthenticateAsync();

            //Act
            var response = await TestClient.DeleteAsync(ApiRoutes.Post.Delete.Replace("{id}", createdPost.Id.ToString()));
            var doubleCheck = await TestClient.GetAsync(ApiRoutes.Post.Get.Replace("{id}", createdPost.Id.ToString()));

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
            doubleCheck.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Delete_ReturnsNoContent_WhenAdminAndPostExists()
        {
            //Arrange
            var createdPost = await CreatePostAsync(new CreatePostRequest
            {
                Title = "New Post",
                Content = "New Content",
                TagNames = new List<string>(),
            });

            await AuthenticateAsAdminAsync();

            //Act
            var response = await TestClient.DeleteAsync(ApiRoutes.Post.Delete.Replace("{id}", createdPost.Id.ToString()));
            var doubleCheck = await TestClient.GetAsync(ApiRoutes.Post.Get.Replace("{id}", createdPost.Id.ToString()));

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
            doubleCheck.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Delete_ReturnsNoAuthorized_WhenNotOwnAndPostExists()
        {
            //Arrange
            var createdPost = await CreatePostAsync(new CreatePostRequest
            {
                Title = "New Post",
                Content = "New Content",
                TagNames = new List<string>(),
            });

            var token = await RegisterUserAsync(new UserRegistrationRequest
            {
                Email = "Test@gmail.com",
                Password = "HelloWorld1234!",
                UserName = "Test"
            });

            AuthenticateWithToken(token);

            //Act
            var response = await TestClient.DeleteAsync(ApiRoutes.Post.Delete.Replace("{id}", createdPost.Id.ToString()));
            var doubleCheck = await TestClient.GetAsync(ApiRoutes.Post.Get.Replace("{id}", createdPost.Id.ToString()));

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
            doubleCheck.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_WhenPostNotExists()
        {
            //Arrange
            await AuthenticateAsync();

            //Act
            var response = await TestClient.DeleteAsync(ApiRoutes.Post.Delete.Replace("{id}", Guid.Empty.ToString()));

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Get_ReturnsPost_WhenAccountAndPostExists()
        {
            //Arrange
            var title = "new Post";
            var content = "new Content";
            var tagNames = new List<string> { "new Tag" };

            var createdPost = await CreatePostAsync(new CreatePostRequest
            {
                Title = title,
                Content = content,
                TagNames = tagNames
            });

            await AuthenticateAsync();

            //Act
            var response = await TestClient.GetAsync(ApiRoutes.Post.Get.Replace("{id}", createdPost.Id.ToString()));

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var returnedPost = await response.Content.ReadAsAsync<Response<PostResponse>>();
            returnedPost.Data.Title.Should().Be(title);
            returnedPost.Data.Content.Should().Be(content);
            returnedPost.Data.Tags.Select(x => x.Name).Should().BeEquivalentTo(tagNames);
        }

        [Fact]
        public async Task Get_ReturnsUnAuthorized_WhenNoAccount()
        {
            //Arrange
            var createdPost = await CreatePostAsync(new CreatePostRequest
            {
                Title = "New Post",
                Content = "New Content",
                TagNames = new List<string> { "new Tag" }
            });

            Logout();

            //Act
            var response = await TestClient.GetAsync(ApiRoutes.Post.Get.Replace("{id}", createdPost.Id.ToString()));

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Get_ReturnsNotFound_WhenAccountAndNoPost()
        {
            //Arrange
            await AuthenticateAsync();

            //Act
            var response = await TestClient.GetAsync(ApiRoutes.Post.Get.Replace("{id}", Guid.Empty.ToString()));

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetAll_ReturnsUnAuthorized_WhenNoAccount()
        {
            //Act
            var response = await TestClient.GetAsync(ApiRoutes.Post.GetAll);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task GetAll_ReturnsNoData_WhenFreshDatabase()
        {
            //Arrange
            await AuthenticateAsync();

            //Act
            var response = await TestClient.GetAsync(ApiRoutes.Post.GetAll);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var returnedPosts = await response.Content.ReadAsAsync<PagedResponse<PostResponse>>();
            returnedPosts.Data.Should().BeEmpty();
        }


        [Fact]
        public async Task GetAll_ReturnsPostOne_WhenApplyFilter()
        {
            //Arrange
            var titleOne = "New PostOne";
            var contentOne = "New ContentOne";
            var tagOne = new List<string>() { "Supreme", "Kind", "New" };

            var createdPostOne = await CreatePostAsync(new CreatePostRequest
            {
                Title = titleOne,
                Content = contentOne,
                TagNames = tagOne
            });

            var createdPostTwo = await CreatePostAsync(new CreatePostRequest
            {
                Title = "New PostTwo",
                Content = "New ContentTwo",
                TagNames = new List<string>() { "Supreme" }
            });

            var filter = new GetAllPostsFilters
            {
                Title = "New Post",
                TagNames = new List<string>() { "Supreme", "Kind" }
            };

            await AuthenticateAsync();

            //Act
            var response = await TestClient.GetAsync(ApiRoutes.Post.GetAll + filter.ToQueryString());

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var returnedPosts = await response.Content.ReadAsAsync<PagedResponse<PostResponse>>();
            returnedPosts.Data.Should().HaveCount(1);
            returnedPosts.Data.First().Title.Should().Be(titleOne);
            returnedPosts.Data.First().Content.Should().Be(contentOne);
            tagOne.Should().BeSubsetOf(returnedPosts.Data.First().Tags.Select(x => x.Name));
        }

        [Fact]
        public async Task Update_ReturnsUnAuthorized_WhenNoAccount()
        {
            var createdPost = await CreatePostAsync(new CreatePostRequest
            {
                Title = "New Post",
                Content = "New Content",
                TagNames = new List<string>()
            });

            //Act
            var response = await TestClient.PatchAsync(ApiRoutes.Post.Update.Replace("{id}", createdPost.Id.ToString()), JsonContent.Create(new UpdatePostReqest
            {
                Content = "Updated Post"
            }));

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Update_ReturnsUnAuthorized_WhenNotOwn()
        {
            //Arrange
            var createdPost = await CreatePostAsync(new CreatePostRequest
            {
                Title = "New Post",
                Content = "New Content",
                TagNames = new List<string>()
            });

            var token = await RegisterUserAsync(new UserRegistrationRequest
            {
                UserName = "SomeUserName",
                Email = "Inte@gmail.com",
                Password = "SomePass1234!",
            });

            AuthenticateWithToken(token);

            //Act
            var response = await TestClient.PatchAsync(ApiRoutes.Post.Update.Replace("{id}", createdPost.Id.ToString()), JsonContent.Create(new UpdatePostReqest
            {
                Title = "Updated Post",
                Content = "Updated Post",
                TagNames = new List<string>()
            }));

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Update_ReturnsUnAuthorized_WhenNotAddmin()
        {
            //Arrange
            var createdPost = await CreatePostAsync(new CreatePostRequest
            {
                Title = "New Post",
                Content = "New Content",
                TagNames = new List<string>()
            });

            await AuthenticateAsAdminAsync();

            //Act
            var response = await TestClient.PatchAsync(ApiRoutes.Post.Update.Replace("{id}", createdPost.Id.ToString()), JsonContent.Create(new UpdatePostReqest
            {
                Title = "Edited By Admin",
                Content = "Updated Post",
                TagNames = new List<string>()
            }));

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Update_ReturnsUpdatedPost_WhenOwnPost()
        {
            //Arrange
            var title = "New Post";
            var newContent = "Updated Post";
            var newTags = new List<string>() { "Test" };

            var createdPost = await CreatePostAsync(new CreatePostRequest
            {
                Title = "New Post",
                Content = "New Content",
                TagNames = new List<string>()
            });

            var createdPostTwo = await CreatePostAsync(new CreatePostRequest
            {
                Title = "New Post",
                Content = "New Content",
                TagNames = newTags
            });

            await AuthenticateAsync();

            //Act
            var response = await TestClient.PatchAsync(ApiRoutes.Post.Update.Replace("{id}", createdPost.Id.ToString()), JsonContent.Create(new UpdatePostReqest
            {
                Title = "New Post",
                Content = newContent,
                TagNames = new List<string>() { "Test" }
            }));

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var returnedPost = await response.Content.ReadAsAsync<Response<PostResponse>>();
            returnedPost.Data.Title.Should().Be(title);
            returnedPost.Data.Content.Should().Be(newContent);

            var doubleCheck = await TestClient.GetAsync(ApiRoutes.Post.Get.Replace("{id}", createdPost.Id.ToString()));
            var doubleCheckPost = await doubleCheck.Content.ReadAsAsync<Response<PostResponse>>();
            doubleCheckPost.Data.Title.Should().Be(title);
            doubleCheckPost.Data.Content.Should().Be(newContent);
            doubleCheckPost.Data.Tags.Select(x => x.Name).Should().BeEquivalentTo(newTags);
        }


        [Fact]
        public async Task UpdateRating_ReturnsUnAuthorized_WhenNoAccount()
        {
            //Arrange
            var createdPost = await CreatePostAsync(new CreatePostRequest
            {
                Title = "New Post",
                Content = "New Content",
                TagNames = new List<string>()
            });

            //Act
            var response = await TestClient.PatchAsync(ApiRoutes.Post.UpdateRating.Replace("{id}", createdPost.Id.ToString()), JsonContent.Create(string.Empty));

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task UpdateRating_ReturnsNotFound_WhenPostNotExists()
        {
            //Arrange
            await AuthenticateAsync();

            //Act
            var response = await TestClient.PatchAsync(ApiRoutes.Post.UpdateRating.Replace("{id}", Guid.Empty.ToString()), JsonContent.Create(string.Empty));

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task UpdateRating_ReturnsUserAddedPost_WhenAccountAndNotLiked()
        {
            //Arrange
            var createdPost = await CreatePostAsync(new CreatePostRequest
            {
                Title = "New Post",
                Content = "New Content",
                TagNames = new List<string>()
            });

            var token = await AuthenticateAsync();

            //Act
            var response = await TestClient.PatchAsync(ApiRoutes.Post.UpdateRating.Replace("{id}", createdPost.Id.ToString()), JsonContent.Create(string.Empty));

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var returnedPost = await response.Content.ReadAsAsync<Response<PostResponse>>();
            returnedPost.Data.UserLikedIds.Should().Contain(GetUserId(token));

            var doubleCheck = await TestClient.GetAsync(ApiRoutes.Post.Get.Replace("{id}", createdPost.Id.ToString()));
            var doubleCheckPost = await doubleCheck.Content.ReadAsAsync<Response<PostResponse>>();
            doubleCheckPost.Data.UserLikedIds.Should().Contain(GetUserId(token));
        }

        [Fact]
        public async Task UpdateRating_ReturnsUserRemovedPost_WhenAccountAndLiked()
        {
            //Arrange
            var createdPost = await CreatePostAsync(new CreatePostRequest
            {
                Title = "New Post",
                Content = "New Content",
                TagNames = new List<string>()
            });

            var token = await AuthenticateAsync();
            await TestClient.PatchAsync(ApiRoutes.Post.UpdateRating.Replace("{id}", createdPost.Id.ToString()), JsonContent.Create(string.Empty));

            //Act
            var response = await TestClient.PatchAsync(ApiRoutes.Post.UpdateRating.Replace("{id}", createdPost.Id.ToString()), JsonContent.Create(string.Empty));

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var returnedPost = await response.Content.ReadAsAsync<Response<PostResponse>>();
            returnedPost.Data.UserLikedIds.Should().NotContain(GetUserId(token));

            var doubleCheck = await TestClient.GetAsync(ApiRoutes.Post.Get.Replace("{id}", createdPost.Id.ToString()));
            var doubleCheckPost = await doubleCheck.Content.ReadAsAsync<Response<PostResponse>>();
            doubleCheckPost.Data.UserLikedIds.Should().NotContain(GetUserId(token));
        }
    }
}
