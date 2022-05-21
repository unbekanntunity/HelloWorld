using FluentAssertions;
using HelloWorldAPI.Contracts.V1;
using HelloWorldAPI.Contracts.V1.Requests;
using HelloWorldAPI.Contracts.V1.Responses;
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
    public class ReplyControllerTests : IntegrationTests
    {
        [Fact]
        public async Task CreateOnArticle_ReturnsUnAuthorized_WhenNoAccount()
        {
            //Arrange
            var createdDiscussion = await CreateDiscussionAsync(new CreateDiscussionRequest
            {
                StartMessage = "StartMessage",
                TagNames = new List<string>(),
                Title = "Title"
            });

            var createdArticle = await CreateArticleAsync(createdDiscussion.Id, new CreateArticleRequest
            {
                Content = "Hello there"
            });

            //Act
            var response = await TestClient.PostAsJsonAsync(ApiRoutes.Reply.CreateOnArticle.Replace("{articleId}", createdArticle.Id.ToString()),
                new CreateReplyRequest
                {
                    Content = "Hello back"
                });

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task CreateOnArticle_ReturnsNotFound_WhenArticleNotExists()
        {
            //Arrange
            await AuthenticateAsync();

            //Act
            var response = await TestClient.PostAsJsonAsync(ApiRoutes.Reply.CreateOnArticle.Replace("{articleId}", Guid.Empty.ToString()),
                new CreateReplyRequest
                {
                    Content = "Hello back"
                });

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task CreateOnArticle_ReturnsCreatedReply_WhenAcountAndExists()
        {
            //Arrange
            var content = "Hello back";

            var createdDiscussion = await CreateDiscussionAsync(new CreateDiscussionRequest
            {
                StartMessage = "StartMessage",
                TagNames = new List<string>(),
                Title = "Title"
            });

            var createdArticle = await CreateArticleAsync(createdDiscussion.Id, new CreateArticleRequest
            {
                Content = "Hello there"
            });

            await AuthenticateAsync();

            //Act
            var response = await TestClient.PostAsJsonAsync(ApiRoutes.Reply.CreateOnArticle.Replace("{articleId}", Guid.Empty.ToString()),
                new CreateReplyRequest
                {
                    Content = content
                });

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            var returnedReply = await response.Content.ReadAsAsync<Response<ReplyResponse>>();
            returnedReply.Data.Content.Should().Be(content);
            returnedReply.Data.CreatedAt.Day.Should().Be(DateTime.UtcNow.Day);
            returnedReply.Data.UpdatedAt.Day.Should().Be(DateTime.UtcNow.Day);

            var doubleCheck = await TestClient.GetAsync(ApiRoutes.Reply.Get.Replace("{id}", returnedReply.Data.Id.ToString()));
            doubleCheck.StatusCode.Should().Be(HttpStatusCode.OK);

            var doubleCheckReply = await doubleCheck.Content.ReadAsAsync<Response<ReplyResponse>>();
            doubleCheckReply.Data.Content.Should().Be(content);
            doubleCheckReply.Data.CreatedAt.Day.Should().Be(DateTime.UtcNow.Day);
            doubleCheckReply.Data.UpdatedAt.Day.Should().Be(DateTime.UtcNow.Day);

            var trippleCheck = await TestClient.GetAsync(ApiRoutes.Article.Get.Replace("{id}", returnedReply.Data.RepliedOnId.ToString()));
            trippleCheck.StatusCode.Should().Be(HttpStatusCode.OK);

            var trippleCheckArticle = await trippleCheck.Content.ReadAsAsync<Response<ArticleResponse>>();
            trippleCheckArticle.Data.Replies.Select(x => x.Id).Should().Contain(returnedReply.Data.Id);
        }
    }
}
