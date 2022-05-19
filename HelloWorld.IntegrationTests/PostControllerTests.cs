using FluentAssertions;
using HelloWorldAPI.Contracts.V1;
using HelloWorldAPI.Contracts.V1.Requests;
using HelloWorldAPI.Contracts.V1.Responses;
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

            var doubleCheck = await TestClient.GetAsync(ApiRoutes.Post.Get.Replace("{id}", returnedPost.Data.Id.ToString()));
            doubleCheck.StatusCode.Should().Be(HttpStatusCode.OK);

            var doubleCheckPost = await doubleCheck.Content.ReadAsAsync<Response<PostResponse>>();
            doubleCheckPost.Data.Title.Should().Be(title);
            doubleCheckPost.Data.Content.Should().Be(content);
            doubleCheckPost.Data.Tags.Select(x => x.Name).Should().BeEquivalentTo(tagNames);
        }
    }
}
