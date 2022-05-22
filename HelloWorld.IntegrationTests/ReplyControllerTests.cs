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
            var response = await TestClient.PostAsJsonAsync(ApiRoutes.Reply.CreateOnArticle.Replace("{id}", createdArticle.Id.ToString()),
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
            var response = await TestClient.PostAsJsonAsync(ApiRoutes.Reply.CreateOnArticle.Replace("{id}", Guid.Empty.ToString()),
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
            var response = await TestClient.PostAsJsonAsync(ApiRoutes.Reply.CreateOnArticle.Replace("{id}", createdArticle.Id.ToString()),
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

        [Fact]
        public async Task CreateOnComment_ReturnsUnAuthorized_WhenNoAccount()
        {
            //Arrange
            var createdPost = await CreatePostAsync(new CreatePostRequest
            {
                Content = "Hello",
                Title = "My first post",
                TagNames = new List<string>()
            });

            var createdComment = await CreateCommentAsync(createdPost.Id, new CreateCommentRequest
            {
                Content = "Cool"
            });

            //Act
            var response = await TestClient.PostAsJsonAsync(ApiRoutes.Reply.CreateOnComment.Replace("{id}", createdComment.Id.ToString()),
                new CreateReplyRequest
                {
                    Content = "Hello back"
                });

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task CreateOnComment_ReturnsNotFound_WhenCommentNotExists()
        {
            //Arrange
            await AuthenticateAsync();

            //Act
            var response = await TestClient.PostAsJsonAsync(ApiRoutes.Reply.CreateOnComment.Replace("{id}", Guid.Empty.ToString()),
                new CreateReplyRequest
                {
                    Content = "Hello back"
                });

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task CreateOnComment_ReturnsCreatedReply_WhenAcountAndExists()
        {
            //Arrange
            var content = "Hello back";

            var createdPost = await CreatePostAsync(new CreatePostRequest
            {
                Content = "Hello",
                Title = "My first post",
                TagNames = new List<string>()
            });

            var createdComment = await CreateCommentAsync(createdPost.Id, new CreateCommentRequest
            {
                Content = "Cool"
            });

            await AuthenticateAsync();

            //Act
            var response = await TestClient.PostAsJsonAsync(ApiRoutes.Reply.CreateOnComment.Replace("{id}", createdComment.Id.ToString()),
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

            var trippleCheck = await TestClient.GetAsync(ApiRoutes.Comment.Get.Replace("{id}", returnedReply.Data.RepliedOnId.ToString()));
            trippleCheck.StatusCode.Should().Be(HttpStatusCode.OK);

            var trippleCheckComment = await trippleCheck.Content.ReadAsAsync<Response<CommentResponse>>();
            trippleCheckComment.Data.Replies.Select(x => x.Id).Should().Contain(returnedReply.Data.Id);
        }

        [Fact]
        public async Task CreateOnReply_ReturnsUnAuthorized_WhenNoAccount()
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

            var createdReply = await CreateReplyAsync(ApiRoutes.Reply.CreateOnArticle, createdArticle.Id, new CreateReplyRequest
            {
                Content = "Hello back"
            });

            //Act
            var response = await TestClient.PostAsJsonAsync(ApiRoutes.Reply.CreateOnReply.Replace("{id}", createdReply.Id.ToString()),
                new CreateReplyRequest
                {
                    Content = "Hello back"
                });

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task CreateOnReply_ReturnsNotFound_WhenReplyNotExists()
        {
            //Arrange
            await AuthenticateAsync();

            //Act
            var response = await TestClient.PostAsJsonAsync(ApiRoutes.Reply.CreateOnReply.Replace("{id}", Guid.Empty.ToString()),
                new CreateReplyRequest
                {
                    Content = "Hello back"
                });

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task CreateOnReply_ReturnsCreatedReply_WhenAcountAndExists()
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

            var createdReply = await CreateReplyAsync(ApiRoutes.Reply.CreateOnArticle, createdArticle.Id, new CreateReplyRequest
            {
                Content = "Hello back"
            });

            await AuthenticateAsync();

            //Act
            var response = await TestClient.PostAsJsonAsync(ApiRoutes.Reply.CreateOnReply.Replace("{id}", createdReply.Id.ToString()),
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

            var trippleCheck = await TestClient.GetAsync(ApiRoutes.Reply.Get.Replace("{id}", returnedReply.Data.RepliedOnId.ToString()));
            trippleCheck.StatusCode.Should().Be(HttpStatusCode.OK);

            var trippleCheckArticle = await trippleCheck.Content.ReadAsAsync<Response<ReplyResponse>>();
            trippleCheckArticle.Data.Replies.Select(x => x.Id).Should().Contain(returnedReply.Data.Id);
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

            var createdReply = await CreateReplyAsync(ApiRoutes.Reply.CreateOnArticle, createdArticle.Id, new CreateReplyRequest
            {
                Content = "Hello back"
            });

            //Act
            var response = await TestClient.DeleteAsync(ApiRoutes.Reply.Delete.Replace("{id}", createdReply.Id.ToString()));

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Delete_ReturnsUnAuthorized_WhenNotOwnReply()
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

            var createdReply = await CreateReplyAsync(ApiRoutes.Reply.CreateOnArticle, createdArticle.Id, new CreateReplyRequest
            {
                Content = "Hello back"
            });

            await AuthenticateAsSecondAsync();

            //Act
            var response = await TestClient.DeleteAsync(ApiRoutes.Reply.Delete.Replace("{id}", createdReply.Id.ToString()));

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_WhenReplyDontExists()
        {
            //Arrange
            await AuthenticateAsync();

            //Act
            var response = await TestClient.DeleteAsync(ApiRoutes.Reply.Delete.Replace("{id}", Guid.Empty.ToString()));

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Delete_ReturnsNoContent_WhenOwnAndReplyExists()
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

            var createdReply = await CreateReplyAsync(ApiRoutes.Reply.CreateOnArticle, createdArticle.Id, new CreateReplyRequest
            {
                Content = "Hello back"
            });

            await AuthenticateAsync();

            //Act
            var response = await TestClient.DeleteAsync(ApiRoutes.Reply.Delete.Replace("{id}", createdReply.Id.ToString()));

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);

            var doubleCheck = await TestClient.GetAsync(ApiRoutes.Reply.Get.Replace("{id}", createdReply.Id.ToString()));
            doubleCheck.StatusCode.Should().Be(HttpStatusCode.NotFound);

            var trippleCheck = await TestClient.GetAsync(ApiRoutes.Article.Get.Replace("{id}", createdArticle.Id.ToString()));
            trippleCheck.StatusCode.Should().Be(HttpStatusCode.OK);

            var trippleCheckArticle = await trippleCheck.Content.ReadAsAsync<Response<ArticleResponse>>();
            trippleCheckArticle.Data.Replies.Select(x => x.Id).Should().NotContain(createdReply.Id);
        }

        [Fact]
        public async Task Delete_ReturnsNoContent_WhenAdminAndReplyExists()
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

            var createdReply = await CreateReplyAsync(ApiRoutes.Reply.CreateOnArticle, createdArticle.Id, new CreateReplyRequest
            {
                Content = "Hello back"
            });

            await AuthenticateAsAdminAsync();

            //Act
            var response = await TestClient.DeleteAsync(ApiRoutes.Reply.Delete.Replace("{id}", createdReply.Id.ToString()));

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);

            var doubleCheck = await TestClient.GetAsync(ApiRoutes.Reply.Get.Replace("{id}", createdReply.Id.ToString()));
            doubleCheck.StatusCode.Should().Be(HttpStatusCode.NotFound);

            var trippleCheck = await TestClient.GetAsync(ApiRoutes.Article.Get.Replace("{id}", createdArticle.Id.ToString()));
            trippleCheck.StatusCode.Should().Be(HttpStatusCode.OK);

            var trippleCheckArticle = await trippleCheck.Content.ReadAsAsync<Response<ArticleResponse>>();
            trippleCheckArticle.Data.Replies.Select(x => x.Id).Should().NotContain(createdReply.Id);
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
                Content = "Hello there"
            });

            var createdReply = await CreateReplyAsync(ApiRoutes.Reply.CreateOnArticle, createdArticle.Id, new CreateReplyRequest
            {
                Content = "Hello back"
            });

            //Act
            var response = await TestClient.GetAsync(ApiRoutes.Reply.Get.Replace("{id}", createdReply.Id.ToString()));

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Get_ReturnsNotFound_WhenReplyNotExists()
        {
            //Arrange
            await AuthenticateAsync();

            //Act
            var response = await TestClient.GetAsync(ApiRoutes.Reply.Get.Replace("{id}", Guid.Empty.ToString()));

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Get_ReturnsReply_WhenReplyExists()
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

            var createdReply = await CreateReplyAsync(ApiRoutes.Reply.CreateOnArticle, createdArticle.Id, new CreateReplyRequest
            {
                Content = content
            });

            await AuthenticateAsync();

            //Act
            var response = await TestClient.GetAsync(ApiRoutes.Reply.Get.Replace("{id}", createdReply.Id.ToString()));

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var returnedReply = await response.Content.ReadAsAsync<Response<ReplyResponse>>();
            returnedReply.Data.Content.Should().Be(content);
        }

        [Fact]
        public async Task GetAll_ReturnsUnAuthorized_WhenNoAccount()
        {
            //Act
            var response = await TestClient.GetAsync(ApiRoutes.Reply.GetAll);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task GetAll_ReturnsZeroData_WhenFreshDatabase()
        {
            //Arrange
            await AuthenticateAsync();

            //Act
            var response = await TestClient.GetAsync(ApiRoutes.Reply.GetAll);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var returnedReplies = await response.Content.ReadAsAsync<PagedResponse<PartialReplyResponse>>();
            returnedReplies.Data.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAll_ReturnsReplyOne_WhenApplyFilter()
        {
            //Arrange
            var contentOne = "Hello from the other side";

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

            var createdReplyOne = await CreateReplyAsync(ApiRoutes.Reply.CreateOnArticle, createdArticle.Id, new CreateReplyRequest
            {
                Content = contentOne
            });

            await CreateReplyAsync(ApiRoutes.Reply.CreateOnArticle, createdArticle.Id, new CreateReplyRequest
            {
                Content = "Hello back"
            });

            var filter = new GetAllRepliesFilter
            {
                Content = "Hello f"
            };

            await AuthenticateAsync();

            //Act
            var response = await TestClient.GetAsync(ApiRoutes.Reply.GetAll + filter.ToQueryString());

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var returnedReplies = await response.Content.ReadAsAsync<PagedResponse<PartialReplyResponse>>();
            returnedReplies.Data.Should().HaveCount(1);
            returnedReplies.Data.First().Content.Should().Be(contentOne);
        }

        [Fact]
        public async Task GetAll_ReturnsCorrectPagination_WhenEmptyData()
        {
            //Arrange
            await AuthenticateAsync();

            //Act
            var response = await TestClient.GetAsync(ApiRoutes.Reply.GetAll);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var returnedComments = await response.Content.ReadAsAsync<PagedResponse<PartialReplyResponse>>();
            returnedComments.NextPage.Should().BeNull();
            returnedComments.PreviousPage.Should().BeNull();
            returnedComments.PageNumber.Should().Be(1);
        }

        [Fact]
        public async Task GetAll_ReturnsCorrectPagination_WhenHaveData()
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

            for (int i = 0; i < 5; i++)
            {
                await CreateReplyAsync(ApiRoutes.Reply.CreateOnArticle, createdArticle.Id, new CreateReplyRequest
                {
                    Content = "Hello there"
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
            var response = await TestClient.GetAsync(ApiRoutes.Reply.GetAll + paginationFilter.ToQueryString());

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var returnedComments = await response.Content.ReadAsAsync<PagedResponse<PartialReplyResponse>>();
            returnedComments.NextPage.Should().Be(GetAllUriNext(ApiRoutes.Reply.GetAll, pageNumber, pageSize));
            returnedComments.PreviousPage.Should().Be(GetAllUriLast(ApiRoutes.Reply.GetAll, pageNumber, pageSize));
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
                Content = "Hello there"
            });

            var createdReply = await CreateReplyAsync(ApiRoutes.Reply.CreateOnArticle, createdArticle.Id, new CreateReplyRequest
            {
                Content = "Hello back"
            });

            //Act
            var response = await TestClient.PatchAsync(ApiRoutes.Reply.Update.Replace("{id}", createdReply.Id.ToString()),
                JsonContent.Create(new UpdateReplyRequest
                {
                    Content = "Hello"
                }));

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Update_ReturnsUnAuthorized_WhenNotOwnReply()
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

            var createdReply = await CreateReplyAsync(ApiRoutes.Reply.CreateOnArticle, createdArticle.Id, new CreateReplyRequest
            {
                Content = "Hello back"
            });

            await AuthenticateAsSecondAsync();

            //Act
            var response = await TestClient.PatchAsync(ApiRoutes.Reply.Update.Replace("{id}", createdReply.Id.ToString()),
                JsonContent.Create(new UpdateReplyRequest
                {
                    Content = "Hello new"
                }));

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Update_ReturnsUnAuthorized_WhenNotOwnReplyEvenWithAdmin()
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

            var createdReply = await CreateReplyAsync(ApiRoutes.Reply.CreateOnArticle, createdArticle.Id, new CreateReplyRequest
            {
                Content = "Hello back"
            });

            await AuthenticateAsAdminAsync();

            //Act
            var response = await TestClient.PatchAsync(ApiRoutes.Reply.Update.Replace("{id}", createdReply.Id.ToString()),
                JsonContent.Create(new UpdateReplyRequest
                {
                    Content = "Hello new"
                }));

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Update_ReturnsNotFound_WhenReplyDontExists()
        {
            //Arrange
            await AuthenticateAsync();

            //Act
            var response = await TestClient.PatchAsync(ApiRoutes.Reply.Update.Replace("{id}", Guid.Empty.ToString()),
                JsonContent.Create(new UpdateReplyRequest
                {
                    Content = "Hello new"
                }));

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Update_ReturnsUpdatedReply_WhenOwnAndExists()
        {
            //Arrange
            var newContent = "Hello back and gm";

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

            var createdReply = await CreateReplyAsync(ApiRoutes.Reply.CreateOnArticle, createdArticle.Id, new CreateReplyRequest
            {
                Content = "Hello back"
            });

            await AuthenticateAsync();

            //Act
            var response = await TestClient.PatchAsync(ApiRoutes.Reply.Update.Replace("{id}", createdReply.Id.ToString()),
                JsonContent.Create(new UpdateReplyRequest
                {
                    Content = newContent
                }));

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var returnedReply = await response.Content.ReadAsAsync<Response<ReplyResponse>>();
            returnedReply.Data.Content.Should().Be(newContent);
            returnedReply.Data.UpdatedAt.Day.Should().Be(DateTime.UtcNow.Day);

            var doubleCheck = await TestClient.GetAsync(ApiRoutes.Reply.Get.Replace("{id}", returnedReply.Data.Id.ToString()));
            doubleCheck.StatusCode.Should().Be(HttpStatusCode.OK);

            var doubleCheckReply = await doubleCheck.Content.ReadAsAsync<Response<ReplyResponse>>();
            doubleCheckReply.Data.Content.Should().Be(newContent);
            doubleCheckReply.Data.UpdatedAt.Day.Should().Be(DateTime.UtcNow.Day);

            var trippleCheck = await TestClient.GetAsync(ApiRoutes.Article.Get.Replace("{id}", returnedReply.Data.RepliedOnId.ToString()));
            trippleCheck.StatusCode.Should().Be(HttpStatusCode.OK);

            var trippleCheckArticle = await trippleCheck.Content.ReadAsAsync<Response<ArticleResponse>>();
            trippleCheckArticle.Data.Replies.Select(x => x.Id).Should().Contain(returnedReply.Data.Id);
            trippleCheckArticle.Data.Replies.First(x => x.Id == returnedReply.Data.Id).Content.Should().Be(newContent);
            trippleCheckArticle.Data.Replies.First(x => x.Id == returnedReply.Data.Id).UpdatedAt.Day.Should().Be(DateTime.UtcNow.Day);
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
                Content = "Hello there"
            });

            var createdReply = await CreateReplyAsync(ApiRoutes.Reply.CreateOnArticle, createdArticle.Id, new CreateReplyRequest
            {
                Content = "Hello back"
            });

            //Act
            var response = await TestClient.PatchAsync(ApiRoutes.Reply.UpdateRating.Replace("{id}", createdReply.Id.ToString()),
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
            var response = await TestClient.PatchAsync(ApiRoutes.Reply.UpdateRating.Replace("{id}", Guid.Empty.ToString()),
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
                Content = "Hello there"
            });

            var createdReply = await CreateReplyAsync(ApiRoutes.Reply.CreateOnArticle, createdArticle.Id, new CreateReplyRequest
            {
                Content = "Hello back"
            });

            var token = await AuthenticateAsync();

            //Act
            var response = await TestClient.PatchAsync(ApiRoutes.Reply.UpdateRating.Replace("{id}", createdReply.Id.ToString()),
                new StringContent(string.Empty));

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var returnedProject = await response.Content.ReadAsAsync<Response<ReplyResponse>>();
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
                Content = "Hello there"
            });

            var createdReply = await CreateReplyAsync(ApiRoutes.Reply.CreateOnArticle, createdArticle.Id, new CreateReplyRequest
            {
                Content = "Hello back"
            });

            var token = await AuthenticateAsync();
            await TestClient.PatchAsync(ApiRoutes.Reply.UpdateRating.Replace("{id}", createdReply.Id.ToString()),
                new StringContent(string.Empty));

            //Act
            var response = await TestClient.PatchAsync(ApiRoutes.Reply.UpdateRating.Replace("{id}", createdReply.Id.ToString()),
                new StringContent(string.Empty));

            //Assert
            var e = await response.Content.ReadAsStringAsync();

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var returnedProject = await response.Content.ReadAsAsync<Response<ReplyResponse>>();
            returnedProject.Data.UserLikedIds.Should().NotContain(GetUserId(token));
        }
    }
}
