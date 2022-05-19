using FluentAssertions;
using HelloWorldAPI.Contracts.V1;
using HelloWorldAPI.Contracts.V1.Requests;
using HelloWorldAPI.Contracts.V1.Responses;
using HelloWorldAPI.Domain.Filters;
using HelloWorldAPI.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace HelloWorld.IntegrationTests
{
    public class DiscussionControllerTests : IntegrationTests
    {
        [Fact]
        public async Task Create_ReturnsUnAuthorized_WhenNoAccount()
        {
            //Act
            var response = await TestClient.PostAsJsonAsync(ApiRoutes.Discussion.Create, new CreateDiscussionRequest
            {
                StartMessage = "Hello World",
                TagNames = new List<string>(),
                Title = "First Discussion"
            });

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Create_ReturnsCreatedDiscussion_WhenAccount()
        {
            //Arrange
            await AuthenticateAsync();

            //Act
            var startMsg = "Hello World";
            var tagNames = new List<string>() { "Tags" };
            var title = "First Discussion";

            var response = await TestClient.PostAsJsonAsync(ApiRoutes.Discussion.Create, new CreateDiscussionRequest
            {
                StartMessage = startMsg,
                TagNames = tagNames,
                Title = title
            });

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            var returnedDiscussion = await response.Content.ReadAsAsync<Response<DiscussionResponse>>();
            returnedDiscussion.Data.StartMessage.Should().Be(startMsg);
            returnedDiscussion.Data.Tags.Select(x => x.Name).Should().BeEquivalentTo(tagNames);
            returnedDiscussion.Data.Title.Should().Be(title);
            returnedDiscussion.Data.UpdatedAt.Day.Should().Be(DateTime.UtcNow.Day);
            returnedDiscussion.Data.CreatedAt.Day.Should().Be(DateTime.UtcNow.Day);

            var doubleCheck = await TestClient.GetAsync(ApiRoutes.Discussion.Get.Replace("{id}", returnedDiscussion.Data.Id.ToString()));
            var doubleCheckDiscussion = await doubleCheck.Content.ReadAsAsync<Response<DiscussionResponse>>();
            doubleCheckDiscussion.Data.StartMessage.Should().Be(startMsg);
            doubleCheckDiscussion.Data.Tags.Select(x => x.Name).Should().BeEquivalentTo(tagNames);
            doubleCheckDiscussion.Data.Title.Should().Be(title);
            doubleCheckDiscussion.Data.UpdatedAt.Day.Should().Be(DateTime.UtcNow.Day);
            doubleCheckDiscussion.Data.CreatedAt.Day.Should().Be(DateTime.UtcNow.Day);
        }

        [Fact]
        public async Task Delete_ReturnsUnAuthorized_WhenNoAccount()
        {
            //Arrange
            var createdDiscussion = await CreateDiscussionAsync(new CreateDiscussionRequest
            {
                StartMessage = "StartMessage",
                TagNames = new List<string>(),
                Title = "Title"
            });

            //Act
            var response = await TestClient.DeleteAsync(ApiRoutes.Discussion.Delete.Replace("{id}", createdDiscussion.Id.ToString()));

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Delete_ReturnsUnAuthorized_WhenNotOwnAndDiscussionExists()
        {
            //Arrange
            var createdDiscussion = await CreateDiscussionAsync(new CreateDiscussionRequest
            {
                StartMessage = "StartMessage",
                TagNames = new List<string>(),
                Title = "Title"
            });

            var token = await RegisterUserAsync(new UserRegistrationRequest
            {
                Email = "Test@gmail.com",
                Password = "HelloWorld1234!",
                UserName = "Test"
            });

            AuthenticateWithToken(token);

            //Act
            var response = await TestClient.DeleteAsync(ApiRoutes.Discussion.Delete.Replace("{id}", createdDiscussion.Id.ToString()));

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_WhenAccountAndDiscussionNotExists()
        {
            //Arrange
            await AuthenticateAsync();

            //Act
            var response = await TestClient.DeleteAsync(ApiRoutes.Discussion.Delete.Replace("{id}", Guid.NewGuid().ToString()));

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Delete_ReturnsNoContent_WhenOwnAndDiscussionExists()
        {
            //Arrange
            var createdDiscussion = await CreateDiscussionAsync(new CreateDiscussionRequest
            {
                StartMessage = "StartMessage",
                TagNames = new List<string>(),
                Title = "Title"
            });

            await AuthenticateAsync();

            //Act
            var response = await TestClient.DeleteAsync(ApiRoutes.Discussion.Delete.Replace("{id}", createdDiscussion.Id.ToString()));

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);

            var doubleCheck = await TestClient.GetAsync(ApiRoutes.Discussion.Get.Replace("{id}", createdDiscussion.Id.ToString()));
            doubleCheck.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Delete_ReturnsNoContent_WheAdminAndDiscussionExists()
        {
            //Arrange
            var createdDiscussion = await CreateDiscussionAsync(new CreateDiscussionRequest
            {
                StartMessage = "StartMessage",
                TagNames = new List<string>(),
                Title = "Title"
            });

            await AuthenticateAsAdminAsync();

            //Act
            var response = await TestClient.DeleteAsync(ApiRoutes.Discussion.Delete.Replace("{id}", createdDiscussion.Id.ToString()));

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);

            var doubleCheck = await TestClient.GetAsync(ApiRoutes.Discussion.Get.Replace("{id}", createdDiscussion.Id.ToString()));
            doubleCheck.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Get_ReturnsUnAuthorized_WhenNoAccount()
        {
            //Arrange
            var createdDiscussion = await CreateDiscussionAsync(new CreateDiscussionRequest
            {
                StartMessage = "StartMessage",
                TagNames = new List<string>(),
                Title = "Title"
            });

            //Act
            var response = await TestClient.GetAsync(ApiRoutes.Discussion.Get.Replace("{id}", createdDiscussion.Id.ToString()));

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Get_ReturnsNotFound_WhenAccountAndDiscussionNotExists()
        {
            //Arrange
            await AuthenticateAsync();

            //Act
            var response = await TestClient.GetAsync(ApiRoutes.Discussion.Get.Replace("{id}", Guid.NewGuid().ToString()));

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Get_ReturnsDiscussion_WhenAccountAndDiscussionExists()
        {
            //Arrange
            var startMsg = "Hello World";
            var tagNames = new List<string>() { "Tags" };
            var title = "First Discussion";

            var createdDiscussion = await CreateDiscussionAsync(new CreateDiscussionRequest
            {
                StartMessage = startMsg,
                TagNames = tagNames,
                Title = title
            });

            await AuthenticateAsync();

            //Act
            var response = await TestClient.GetAsync(ApiRoutes.Discussion.Get.Replace("{id}", createdDiscussion.Id.ToString()));

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var returnedDiscussion = await response.Content.ReadAsAsync<Response<DiscussionResponse>>();
            returnedDiscussion.Data.StartMessage.Should().Be(startMsg);
            returnedDiscussion.Data.Tags.Select(x => x.Name).Should().BeEquivalentTo(tagNames);
            returnedDiscussion.Data.Title.Should().Be(title);
        }

        [Fact]
        public async Task GetAll_ReturnsUnAuthorized_WhenFreshDatabase()
        {
            //Act
            var response = await TestClient.GetAsync(ApiRoutes.Discussion.GetAll);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task GetAll_ReturnsZeroData_WhenFreshDatabase()
        {
            //Arrange
            await AuthenticateAsync();

            //Act
            var response = await TestClient.GetAsync(ApiRoutes.Discussion.GetAll);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var returnedDiscussions = await response.Content.ReadAsAsync<PagedResponse<PartialDiscussionResponse>>();
            returnedDiscussions.Data.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAll_ReturnsDiscussionOne_WhenApplyFilter()
        {
            //Arrange
            var title = "My";
            var startMsg = "Hello Guys";
            var tagOne = new List<string>() { "Supreme", "Kind", "New" };

            var createdDiscussionOne = await CreateDiscussionAsync(new CreateDiscussionRequest
            {
                Title = title,
                StartMessage = startMsg,
                TagNames = tagOne
            });

            var createdDiscussionTwo = await CreateDiscussionAsync(new CreateDiscussionRequest
            {
                Title = "New DiscussionTwo",
                StartMessage = "New ContentTwo",
                TagNames = new List<string>() { "Supreme" }
            });

            var filter = new GetAllDiscussionsFilter
            {
                Title = "M",
                TagNames = new List<string>() { "Supreme", "Kind" }
            };

            await AuthenticateAsync();

            //Act
            var response = await TestClient.GetAsync(ApiRoutes.Discussion.GetAll + filter.ToQueryString());

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var returnedDiscussions = await response.Content.ReadAsAsync<PagedResponse<PartialDiscussionResponse>>();
            returnedDiscussions.Data.Should().HaveCount(1);
            returnedDiscussions.Data.First().Title.Should().Be(title);
            returnedDiscussions.Data.First().StartMessage.Should().Be(startMsg);
            tagOne.Should().BeSubsetOf(returnedDiscussions.Data.First().Tags.Select(x => x.Name));
        }

        [Fact]
        public async Task Update_ReturnsUnAuthorized_WhenNoAccount()
        {
            //Assert
            var createdDiscussion = await CreateDiscussionAsync(new CreateDiscussionRequest
            {
                StartMessage = "StartMessage",
                TagNames = new List<string>(),
                Title = "Title"
            });

            //Act
            var response = await TestClient.PatchAsync(ApiRoutes.Discussion.Update.Replace("{id}", createdDiscussion.Id.ToString()), 
                JsonContent.Create(new UpdateDiscussionRequest
                {
                    Tags = new List<string>()
                }));

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Update_ReturnsUnAuthorized_WhenNotOwnDiscussion()
        {
            //Assert
            var createdDiscussion = await CreateDiscussionAsync(new CreateDiscussionRequest
            {
                StartMessage = "StartMessage",
                TagNames = new List<string>(),
                Title = "Title"
            });

            var token = await RegisterUserAsync(new UserRegistrationRequest
            {
                Email = "Test@gmail.com",
                Password = "HelloWorld1234!",
                UserName = "Test"
            });

            AuthenticateWithToken(token);

            //Act
            var response = await TestClient.PatchAsync(ApiRoutes.Discussion.Update.Replace("{id}", createdDiscussion.Id.ToString()),
                JsonContent.Create(new UpdateDiscussionRequest
                {
                    Tags = new List<string>()
                }));

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Update_ReturnsNotFound_WhenDiscussionNotExists()
        {
            //Assert
            await AuthenticateAsync();

            //Act
            var response = await TestClient.PatchAsync(ApiRoutes.Discussion.Update.Replace("{id}", Guid.NewGuid().ToString()),
                JsonContent.Create(new UpdateDiscussionRequest
                {
                    Tags = new List<string>()
                }));

            var e = response.Content.ReadAsStringAsync();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Update_ReturnsUpdatedDiscussion_WhenOwnDiscussionAndExists()
        {
            //Assert
            var newTags = new List<string>() { "Hello" };

            var createdDiscussion = await CreateDiscussionAsync(new CreateDiscussionRequest
            {
                StartMessage = "StartMessage",
                TagNames = new List<string>(),
                Title = "Title"
            });

            await AuthenticateAsync();

            //Act
            var response = await TestClient.PatchAsync(ApiRoutes.Discussion.Update.Replace("{id}", createdDiscussion.Id.ToString()),
                JsonContent.Create(new UpdateDiscussionRequest
                {
                    Tags = newTags
                }));

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var returnedDiscussion = await response.Content.ReadAsAsync<Response<DiscussionResponse>>();
            returnedDiscussion.Data.Tags.Select(x => x.Name).Should().BeEquivalentTo(newTags);

            var doubleCheck = await TestClient.GetAsync(ApiRoutes.Discussion.Get.Replace("{id}", createdDiscussion.Id.ToString()));
            var doubleCheckDiscussion = await doubleCheck.Content.ReadAsAsync<Response<DiscussionResponse>>();
            doubleCheckDiscussion.Data.Tags.Select(x => x.Name).Should().BeEquivalentTo(newTags);
        }
    }
}
