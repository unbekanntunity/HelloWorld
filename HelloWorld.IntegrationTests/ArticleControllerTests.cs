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
    public class ArticleControllerTests : IntegrationTests
    {
        [Fact]
        public async Task Create_ReturnsUnAuthorized_WhenNoAccount()
        {
            var createdDiscussion = await CreateDiscussionAsync(new CreateDiscussionRequest
            {
                StartMessage = "StartMessage",
                TagNames = new List<string>(),
                Title = "Title"
            });

            //Act
            var response = await TestClient.PostAsJsonAsync(ApiRoutes.Article.Create.Replace("{discussionId}", createdDiscussion.Id.ToString()),
                new CreateArticleRequest
                {
                    Content = "Hello"
                });

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Create_ReturnsNotFound_WhenDiscussionNotExists()
        {
            //Arrange
            await AuthenticateAsync();

            //Act
            var response = await TestClient.PostAsJsonAsync(ApiRoutes.Article.Create.Replace("{discussionId}", Guid.Empty.ToString()),
                new CreateArticleRequest
                {
                    Content = "Discussion not exists :("
                });

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Create_ReturnsCreatedArticle_WhenAccount()
        {
            //Arrange
            var content = "HelloWorld is the best social media app";

            var createdDiscussion = await CreateDiscussionAsync(new CreateDiscussionRequest
            {
                StartMessage = "StartMessage",
                TagNames = new List<string>(),
                Title = "Title"
            });

            await AuthenticateAsync();

            //Act
            var response = await TestClient.PostAsJsonAsync(ApiRoutes.Article.Create.Replace("{discussionId}", createdDiscussion.Id.ToString()),
                new CreateArticleRequest
                {
                    Content = content
                });

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            var returnedArticle = await response.Content.ReadAsAsync<Response<ArticleResponse>>();
            returnedArticle.Data.Content.Should().Be(content);
            returnedArticle.Data.CreatedAt.Day.Should().Be(DateTime.UtcNow.Day);
            returnedArticle.Data.UpdatedAt.Day.Should().Be(DateTime.UtcNow.Day);

            var doubleCheck = await TestClient.GetAsync(ApiRoutes.Article.Get.Replace("{id}", returnedArticle.Data.Id.ToString()));
            doubleCheck.StatusCode.Should().Be(HttpStatusCode.OK);

            var doubleCheckArticle = await doubleCheck.Content.ReadAsAsync<Response<ArticleResponse>>();
            doubleCheckArticle.Data.Content.Should().Be(content);
            doubleCheckArticle.Data.CreatedAt.Day.Should().Be(DateTime.UtcNow.Day);
            doubleCheckArticle.Data.UpdatedAt.Day.Should().Be(DateTime.UtcNow.Day);

            var trippleCheck = await TestClient.GetAsync(ApiRoutes.Discussion.Get.Replace("{id}", returnedArticle.Data.DiscussionId.ToString()));
            trippleCheck.StatusCode.Should().Be(HttpStatusCode.OK);

            var trippleCheckDiscussion = await trippleCheck.Content.ReadAsAsync<Response<DiscussionResponse>>();
            trippleCheckDiscussion.Data.Articles.Select(x => x.Id).Should().Contain(returnedArticle.Data.Id);
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

            var createdArticle = await CreateArticleAsync(createdDiscussion.Id, new CreateArticleRequest
            {
                Content = "Hello there"
            });

            //Act
            var response = await TestClient.DeleteAsync(ApiRoutes.Article.Delete.Replace("{id}", createdArticle.Id.ToString()));

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Delete_ReturnsUnAuthorized_WhenNotOwnArticleOrAdmin()
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
                Content = "Hello"
            });

            await AuthenticateAsSecondAsync();

            //Act
            var response = await TestClient.DeleteAsync(ApiRoutes.Article.Delete.Replace("{id}", createdArticle.Id.ToString()));

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_WhenArticleNotExists()
        {
            //Arrange
            await AuthenticateAsync();

            //Act
            var response = await TestClient.DeleteAsync(ApiRoutes.Article.Delete.Replace("{id}", Guid.Empty.ToString()));

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Delete_ReturnsNoContent_WhenOwnArticle()
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
                Content = "Hello"
            });

            await AuthenticateAsync();

            //Act
            var response = await TestClient.DeleteAsync(ApiRoutes.Article.Delete.Replace("{id}", createdArticle.Id.ToString()));

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);

            var doubleCheck = await TestClient.GetAsync(ApiRoutes.Article.Get.Replace("{id}", createdArticle.Id.ToString()));
            doubleCheck.StatusCode.Should().Be(HttpStatusCode.NotFound);

            var trippleCheck = await TestClient.GetAsync(ApiRoutes.Discussion.Get.Replace("{id}", createdDiscussion.Id.ToString()));
            trippleCheck.StatusCode.Should().Be(HttpStatusCode.OK);

            var trippleCheckDisucssion = await trippleCheck.Content.ReadAsAsync<Response<DiscussionResponse>>();
            trippleCheckDisucssion.Data.Articles.Select(x => x.Id).Should().NotContain(createdArticle.Id);
        }

        [Fact]
        public async Task Delete_ReturnsNoContent_WhenAdminAccount()
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
                Content = "Hello"
            });

            await AuthenticateAsAdminAsync();

            //Act
            var response = await TestClient.DeleteAsync(ApiRoutes.Article.Delete.Replace("{id}", createdArticle.Id.ToString()));

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);

            var doubleCheck = await TestClient.GetAsync(ApiRoutes.Article.Get.Replace("{id}", createdArticle.Id.ToString()));
            doubleCheck.StatusCode.Should().Be(HttpStatusCode.NotFound);

            var trippleCheck = await TestClient.GetAsync(ApiRoutes.Discussion.Get.Replace("{id}", createdDiscussion.Id.ToString()));
            trippleCheck.StatusCode.Should().Be(HttpStatusCode.OK);

            var trippleCheckDisucssion = await trippleCheck.Content.ReadAsAsync<Response<DiscussionResponse>>();
            trippleCheckDisucssion.Data.Articles.Select(x => x.Id).Should().NotContain(createdArticle.Id);
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

            var createdArticle = await CreateArticleAsync(createdDiscussion.Id, new CreateArticleRequest
            {
                Content = "Hello"
            });

            //Act
            var response = await TestClient.GetAsync(ApiRoutes.Article.Get.Replace("{id}", createdArticle.Id.ToString()));

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Get_ReturnsNotFound_WhenArticleDontExists()
        {
            //Arrange
            await AuthenticateAsync();

            //Act
            var response = await TestClient.GetAsync(ApiRoutes.Article.Get.Replace("{id}", Guid.Empty.ToString()));

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Get_ReturnsArticle_WhenAccountAndArticleExists()
        {
            //Arrange
            var content = "content";

            var createdDiscussion = await CreateDiscussionAsync(new CreateDiscussionRequest
            {
                StartMessage = "StartMessage",
                TagNames = new List<string>(),
                Title = "Title"
            });

            var createdArticle = await CreateArticleAsync(createdDiscussion.Id, new CreateArticleRequest
            {
                Content = content
            });

            await AuthenticateAsync();

            //Act
            var response = await TestClient.GetAsync(ApiRoutes.Article.Get.Replace("{id}", createdArticle.Id.ToString()));

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var returnedArticle = await response.Content.ReadAsAsync<Response<ArticleResponse>>();
            returnedArticle.Data.Content.Should().Be(content);
        }

        [Fact]
        public async Task GetAll_ReturnsUnAuthorized_WhenNoAccount()
        {
            //Act
            var response = await TestClient.GetAsync(ApiRoutes.Article.GetAll);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task GetAll_ReturnsZeroData_WhenFreshDatabase()
        {
            //Arrange
            await AuthenticateAsync();

            //Act
            var response = await TestClient.GetAsync(ApiRoutes.Article.GetAll);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var returnedArticles = await response.Content.ReadAsAsync<PagedResponse<ArticleResponse>>();
            returnedArticles.Data.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAll_ReturnsArticleOne_WhenApplyFilter()
        {
            //Arrange
            var contentOne = "Hello one";

            var createdDiscussion = await CreateDiscussionAsync(new CreateDiscussionRequest
            {
                StartMessage = "StartMessage",
                TagNames = new List<string>(),
                Title = "Title"
            });

            var createdArticleOne = await CreateArticleAsync(createdDiscussion.Id, new CreateArticleRequest
            {
                Content = contentOne
            });

            await CreateArticleAsync(createdDiscussion.Id, new CreateArticleRequest
            {
                Content = "Hello two"
            });

            var filter = new GetAllArticlesFilter
            {
                Content = "Hello o",
                DiscussionId = createdDiscussion.Id
            };

            await AuthenticateAsync();

            //Act
            var response = await TestClient.GetAsync(ApiRoutes.Article.GetAll + filter.ToQueryString());

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var returnedArticles = await response.Content.ReadAsAsync<PagedResponse<ArticleResponse>>();
            returnedArticles.Data.Should().HaveCount(1);
            returnedArticles.Data.First().Content.Should().Be(contentOne);
            returnedArticles.Data.First().CreatedAt.Day.Should().Be(DateTime.UtcNow.Day);
        }

        [Fact]
        public async Task GetAll_ReturnsCorrectPagination_WhenEmptyData()
        {
            //Arrange
            await AuthenticateAsync();

            //Act
            var response = await TestClient.GetAsync(ApiRoutes.Article.GetAll);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var returnedComments = await response.Content.ReadAsAsync<PagedResponse<ArticleResponse>>();
            returnedComments.NextPage.Should().BeNull();
            returnedComments.PreviousPage.Should().BeNull();
            returnedComments.PageNumber.Should().Be(1);
        }

        [Fact]
        public async Task GetAll_ReturnsCorrectPagination_WhenHaveData()
        {
            //Arrange
            var content = "Hello";
            var createdDiscussion = await CreateDiscussionAsync(new CreateDiscussionRequest
            {
                StartMessage = "Hello there",
                Title = "My first discussion",
                TagNames = new List<string>()
            });

            for (int i = 0; i < 5; i++)
            {
                await CreateArticleAsync(createdDiscussion.Id, new CreateArticleRequest
                {
                    Content = content
                });
            }

            var pageNumber = 2;
            var pageSize = 1;
            var paginationFilter = new PaginationFilter
            {
                PageNumber = 2,
                PageSize = 1
            };

            await AuthenticateAsync();

            //Act
            var response = await TestClient.GetAsync(ApiRoutes.Article.GetAll + paginationFilter.ToQueryString());

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var returnedComments = await response.Content.ReadAsAsync<PagedResponse<ArticleResponse>>();
            returnedComments.NextPage.Should().Be(GetAllUriNext(ApiRoutes.Article.GetAll, pageNumber, pageSize));
            returnedComments.PreviousPage.Should().Be(GetAllUriLast(ApiRoutes.Article.GetAll, pageNumber, pageSize));
            returnedComments.PageNumber.Should().Be(pageNumber);
        }

        [Fact]
        public async Task Update_ReturnsUnAuthorized_WhenNoAccount()
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
                Content = "Hello"
            });

            //Act
            var response = await TestClient.PatchAsync(ApiRoutes.Article.Update.Replace("{id}", createdArticle.Id.ToString()),
                JsonContent.Create(new UpdateArticleRequest
                {
                    Content = "Updated content"
                }));

            //Arrange
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Update_ReturnsUnAuthorized_WhenNotOwnArticle()
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
                Content = "Hello"
            });

            await AuthenticateAsSecondAsync();

            //Act
            var response = await TestClient.PatchAsync(ApiRoutes.Article.Update.Replace("{id}", createdArticle.Id.ToString()),
              JsonContent.Create(new UpdateArticleRequest
              {
                  Content = "Hello"
              }));

            //Arrange
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Update_ReturnsUnAuthorized_WhenNotOwnArticleEvenWithAdmin()
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
                Content = "Hello"
            });

            await AuthenticateAsAdminAsync();

            //Act
            var response = await TestClient.PatchAsync(ApiRoutes.Article.Update.Replace("{id}", createdArticle.Id.ToString()),
              JsonContent.Create(new UpdateArticleRequest
              {
                  Content = "Hello"
              }));

            //Arrange
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Update_ReturnsNotFound_WhenArticleNotExists()
        {
            //Arrange
            await AuthenticateAsync();

            //Act
            var response = await TestClient.PatchAsync(ApiRoutes.Article.Update.Replace("{id}", Guid.Empty.ToString()),
              JsonContent.Create(new UpdateArticleRequest
              {
                  Content = "Hello"
              }));

            //Arrange
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Update_ReturnsUpdatedArticle_WhenOwnAndArticleExists()
        {
            //Arrange
            var newContent = "Updated content";
            var createdDiscussion = await CreateDiscussionAsync(new CreateDiscussionRequest
            {
                StartMessage = "StartMessage",
                TagNames = new List<string>(),
                Title = "Title"
            });

            var createdArticle = await CreateArticleAsync(createdDiscussion.Id, new CreateArticleRequest
            {
                Content = "Hello"
            });

            await AuthenticateAsync();

            //Act
            var response = await TestClient.PatchAsync(ApiRoutes.Article.Update.Replace("{id}", createdArticle.Id.ToString()),
              JsonContent.Create(new UpdateArticleRequest
              {
                  Content = newContent
              }));

            //Arrange
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var returnedArticle = await response.Content.ReadAsAsync<Response<ArticleResponse>>();
            returnedArticle.Data.Content.Should().Be(newContent);
            returnedArticle.Data.UpdatedAt.Day.Should().Be(DateTime.UtcNow.Day);

            var doubleCheck = await TestClient.GetAsync(ApiRoutes.Article.Get.Replace("{id}", returnedArticle.Data.Id.ToString()));
            doubleCheck.StatusCode.Should().Be(HttpStatusCode.OK);

            var doubleCheckArticle = await doubleCheck.Content.ReadAsAsync<Response<ArticleResponse>>();
            doubleCheckArticle.Data.Content.Should().Be(newContent);
            doubleCheckArticle.Data.UpdatedAt.Day.Should().Be(DateTime.UtcNow.Day);

            var trippleCheck = await TestClient.GetAsync(ApiRoutes.Discussion.Get.Replace("{id}", createdArticle.DiscussionId.ToString()));
            trippleCheck.StatusCode.Should().Be(HttpStatusCode.OK);

            var trippleCheckDiscussion = await trippleCheck.Content.ReadAsAsync<Response<DiscussionResponse>>();
            trippleCheckDiscussion.Data.Articles.Select(x => x.Id).Should().Contain(returnedArticle.Data.Id);
            trippleCheckDiscussion.Data.Articles.First(x => x.Id == returnedArticle.Data.Id).Content.Should().Be(newContent);
            trippleCheckDiscussion.Data.Articles.First(x => x.Id == returnedArticle.Data.Id).UpdatedAt.Day.Should().Be(DateTime.UtcNow.Day);
        }

        [Fact]
        public async Task UpdateRating_ReturnsUnAuthorized_WhenNoAccount()
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
                Content = "Hello"
            });

            //Act
            var response = await TestClient.PatchAsync(ApiRoutes.Article.UpdateRating.Replace("{id}", createdArticle.Id.ToString()),
                new StringContent(string.Empty));

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task UpdateRating_ReturnsNotFound_WhenArticleNotExists()
        {
            //Arrange
            await AuthenticateAsync();

            //Act
            var response = await TestClient.PatchAsync(ApiRoutes.Article.UpdateRating.Replace("{id}", Guid.Empty.ToString()),
                new StringContent(string.Empty));

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task UpdateRating_ReturnsUserAdded_WhenAccountAndNotLikedYet()
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
                Content = "Hello"
            });

            var token = await AuthenticateAsync();

            //Act
            var response = await TestClient.PatchAsync(ApiRoutes.Article.UpdateRating.Replace("{id}", createdArticle.Id.ToString()),
                new StringContent(string.Empty));

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var returnedProject = await response.Content.ReadAsAsync<Response<ArticleResponse>>();
            returnedProject.Data.UserLikedIds.Should().Contain(GetUserId(token));
        }

        [Fact]
        public async Task UpdateRating_ReturnsUserRemoved_WhenAccountAndAlreadyLiked()
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
                Content = "Hello"
            });

            var token = await AuthenticateAsync();
            await TestClient.PatchAsync(ApiRoutes.Project.UpdateRating.Replace("{id}", createdArticle.Id.ToString()), new StringContent(string.Empty));

            //Act
            var response = await TestClient.PatchAsync(ApiRoutes.Article.UpdateRating.Replace("{id}", createdArticle.Id.ToString()),
                new StringContent(string.Empty));

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var returnedProject = await response.Content.ReadAsAsync<Response<ArticleResponse>>();
            returnedProject.Data.UserLikedIds.Should().Contain(GetUserId(token));
        }
    }
}
