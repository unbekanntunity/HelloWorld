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
    public class CommentControllerTests : IntegrationTests
    {
        [Fact]
        public async Task Create_ReturnsUnAuthorized_WhenNoAccount()
        {
            //Arrange
            var createdPost = await CreatePostAsync(new CreatePostRequest
            {
                Content = "Hello",
                Title = "My first post",
                TagNames = new List<string>()
            });

            //Act
            var response = await TestClient.PostAsJsonAsync(ApiRoutes.Comment.Create.Replace("{postId}", createdPost.Id.ToString()),
                new CreateCommentRequest
                {
                    Content = "Cool"
                });

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Create_ReturnsNotFound_WhenPostDontExists()
        {
            //Arrange
            await AuthenticateAsync();

            //Act
            var response = await TestClient.PostAsJsonAsync(ApiRoutes.Comment.Create.Replace("{postId}", Guid.Empty.ToString()),
                new CreateCommentRequest
                {
                    Content = "Cool"
                });

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Create_ReturnsCreatedComment_WhenAccountAndPostExists()
        {
            //Arrange
            var content = "Cool";

            var createdPost = await CreatePostAsync(new CreatePostRequest
            {
                Content = "Hello",
                Title = "My first post",
                TagNames = new List<string>()
            });

            await AuthenticateAsync();

            //Act
            var response = await TestClient.PostAsJsonAsync(ApiRoutes.Comment.Create.Replace("{postId}", createdPost.Id.ToString()),
                new CreateCommentRequest
                {
                    Content = content
                });

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            var returnedComment = await response.Content.ReadAsAsync<Response<CommentResponse>>();
            returnedComment.Data.Content.Should().Be(content);
            returnedComment.Data.CreatedAt.Day.Should().Be(DateTime.UtcNow.Day);
            returnedComment.Data.UpdatedAt.Day.Should().Be(DateTime.UtcNow.Day);

            var doubleCheck = await TestClient.GetAsync(ApiRoutes.Comment.Get.Replace("{id}", returnedComment.Data.Id.ToString()));
            doubleCheck.StatusCode.Should().Be(HttpStatusCode.OK);

            var doubleCheckComment = await doubleCheck.Content.ReadAsAsync<Response<CommentResponse>>();
            doubleCheckComment.Data.Content.Should().Be(content);
            doubleCheckComment.Data.CreatedAt.Day.Should().Be(DateTime.UtcNow.Day);
            doubleCheckComment.Data.UpdatedAt.Day.Should().Be(DateTime.UtcNow.Day);

            var tripppleCheck = await TestClient.GetAsync(ApiRoutes.Post.Get.Replace("{id}", createdPost.Id.ToString()));
            tripppleCheck.StatusCode.Should().Be(HttpStatusCode.OK);

            var trippleCheckPost = await tripppleCheck.Content.ReadAsAsync<Response<PostResponse>>();
            trippleCheckPost.Data.Comments.Select(x => x.Id).Should().Contain(returnedComment.Data.Id);
        }

        [Fact]
        public async Task Delete_ReturnsUnAuthorized_WhenNoAccount()
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
            var response = await TestClient.DeleteAsync(ApiRoutes.Comment.Delete.Replace("{id}", createdComment.Id.ToString()));

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Delete_ReturnsUnAuthorized_WhenNotOwnCommentOrAdmin()
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

            await AuthenticateAsSecondAsync();

            //Act
            var response = await TestClient.DeleteAsync(ApiRoutes.Comment.Delete.Replace("{id}", createdComment.Id.ToString()));

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_WhenAccountAndCommentNotExists()
        {
            //Arrange
            await AuthenticateAsync();

            //Act
            var response = await TestClient.DeleteAsync(ApiRoutes.Comment.Delete.Replace("{id}", Guid.Empty.ToString()));

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Delete_ReturnsNoContent_WhenOwnComment()
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

            await AuthenticateAsync();

            //Act
            var response = await TestClient.DeleteAsync(ApiRoutes.Comment.Delete.Replace("{id}", createdComment.Id.ToString()));

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);

            var doubleCheck = await TestClient.GetAsync(ApiRoutes.Comment.Get.Replace("{id}", createdComment.Id.ToString()));
            doubleCheck.StatusCode.Should().Be(HttpStatusCode.NotFound);

            var trippleCheck = await TestClient.GetAsync(ApiRoutes.Post.Get.Replace("{id}", createdPost.Id.ToString()));
            trippleCheck.StatusCode.Should().Be(HttpStatusCode.OK);

            var trippleCheckPost = await trippleCheck.Content.ReadAsAsync<Response<PostResponse>>();
            trippleCheckPost.Data.Comments.Select(x => x.Id).Should().NotContain(createdComment.Id);
        }

        [Fact]
        public async Task Delete_ReturnsNoContent_WhenAdminAccount()
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

            await AuthenticateAsAdminAsync();

            //Act
            var response = await TestClient.DeleteAsync(ApiRoutes.Comment.Delete.Replace("{id}", createdComment.Id.ToString()));

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);

            var doubleCheck = await TestClient.GetAsync(ApiRoutes.Comment.Get.Replace("{id}", createdComment.Id.ToString()));
            doubleCheck.StatusCode.Should().Be(HttpStatusCode.NotFound);

            var trippleCheck = await TestClient.GetAsync(ApiRoutes.Post.Get.Replace("{id}", createdPost.Id.ToString()));
            trippleCheck.StatusCode.Should().Be(HttpStatusCode.OK);

            var trippleCheckPost = await trippleCheck.Content.ReadAsAsync<Response<PostResponse>>();
            trippleCheckPost.Data.Comments.Select(x => x.Id).Should().NotContain(createdComment.Id);
        }

        [Fact]
        public async Task Get_ReturnsUnAuthorized_WhenNoAccount()
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
            var response = await TestClient.GetAsync(ApiRoutes.Comment.Get.Replace("{id}", createdComment.Id.ToString()));

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Get_ReturnsNotFound_WhenCommentNotExists()
        {
            //Arrange
            await AuthenticateAsync();

            //Act
            var response = await TestClient.GetAsync(ApiRoutes.Comment.Get.Replace("{id}", Guid.Empty.ToString()));

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Get_ReturnsComment_WhenCommentExists()
        {
            //Arrange
            var content = "Cool";
            var createdPost = await CreatePostAsync(new CreatePostRequest
            {
                Content = content,
                Title = "My first post",
                TagNames = new List<string>()
            });

            var createdComment = await CreateCommentAsync(createdPost.Id, new CreateCommentRequest
            {
                Content = content
            });

            await AuthenticateAsync();

            //Act
            var response = await TestClient.GetAsync(ApiRoutes.Comment.Get.Replace("{id}", createdComment.Id.ToString()));

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var returnedComment = await response.Content.ReadAsAsync<Response<CommentResponse>>();
            returnedComment.Data.Content.Should().Be(content);
        }

        [Fact]
        public async Task GetAll_ReturnsUnAuthorized_WhenNoAccount()
        {
            //Act
            var response = await TestClient.GetAsync(ApiRoutes.Comment.GetAll);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task GetAll_ReturnsCorrectPagination_WhenEmptyData()
        {
            //Arrange
            await AuthenticateAsync();

            //Act
            var response = await TestClient.GetAsync(ApiRoutes.Comment.GetAll);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var returnedComments = await response.Content.ReadAsAsync<PagedResponse<CommentResponse>>();
            returnedComments.NextPage.Should().BeNull();
            returnedComments.PreviousPage.Should().BeNull();
            returnedComments.PageNumber.Should().Be(1);
        }

        [Fact]
        public async Task GetAll_ReturnsCorrectPagination_WhenHaveData()
        {
            //Arrange
            var content = "Hello";
            var createdPost = await CreatePostAsync(new CreatePostRequest
            {
                Content = content,
                Title = "My first post",
                TagNames = new List<string>()
            });

            await CreateCommentAsync(createdPost.Id, new CreateCommentRequest
            {
                Content = content
            });

            await CreateCommentAsync(createdPost.Id, new CreateCommentRequest
            {
                Content = content
            });

            await CreateCommentAsync(createdPost.Id, new CreateCommentRequest
            {
                Content = content
            });

            await CreateCommentAsync(createdPost.Id, new CreateCommentRequest
            {
                Content = content
            });

            var pageNumber = 2;
            var pageSize = 1;
            var paginationFilter = new PaginationFilter
            {
                PageNumber = 2,
                PageSize = 1
            };

            await AuthenticateAsync();

            //Act
            var response = await TestClient.GetAsync(ApiRoutes.Comment.GetAll + paginationFilter.ToQueryString());

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var returnedComments = await response.Content.ReadAsAsync<PagedResponse<CommentResponse>>();
            returnedComments.NextPage.Should().Be(GetAllUriNext(ApiRoutes.Comment.GetAll, pageNumber, pageSize));
            returnedComments.PreviousPage.Should().Be(GetAllUriLast(ApiRoutes.Comment.GetAll, pageNumber, pageSize));
            returnedComments.PageNumber.Should().Be(pageNumber);
        }

        [Fact]
        public async Task GetAll_ReturnsZeroData_WhenFreshDatabase()
        {
            //Arrange
            await AuthenticateAsync();

            //Act
            var response = await TestClient.GetAsync(ApiRoutes.Comment.GetAll);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var returnedComments = await response.Content.ReadAsAsync<PagedResponse<CommentResponse>>();
            returnedComments.Data.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAll_ReturnsCommentOne_WhenApplyFilter()
        {
            //Arrange
            var contentOne = "Cool";
            var createdPost = await CreatePostAsync(new CreatePostRequest
            {
                Content = "Hello",
                Title = "My first post",
                TagNames = new List<string>()
            });

            var createdcommentOne = await CreateCommentAsync(createdPost.Id, new CreateCommentRequest
            {
                Content = contentOne
            });

            await CreateCommentAsync(createdPost.Id, new CreateCommentRequest
            {
                Content = "Agree"
            });

            var filter = new GetAllCommentsFilter
            {
                Content = "Cool"
            };

            await AuthenticateAsync();

            //Act
            var response = await TestClient.GetAsync(ApiRoutes.Comment.GetAll + filter.ToQueryString());

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var returnedComments = await response.Content.ReadAsAsync<PagedResponse<CommentResponse>>();
            returnedComments.Data.First().Content.Should().Be(contentOne);
            returnedComments.Data.First().CreatedAt.Day.Should().Be(DateTime.UtcNow.Day);
        }

        [Fact]
        public async Task Update_ReturnsUnAuthorized_WhenNoAccount()
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
            var response = await TestClient.PatchAsync(ApiRoutes.Comment.Update.Replace("{id}", createdComment.Id.ToString()),
                JsonContent.Create(new UpdateCommentRequest
                {
                    Content = "New cool"
                }));

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Update_ReturnsUnAuthorized_WhenNotOwnComment()
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

            await AuthenticateAsSecondAsync();

            //Act
            var response = await TestClient.PatchAsync(ApiRoutes.Comment.Update.Replace("{id}", createdComment.Id.ToString()),
                JsonContent.Create(new UpdateCommentRequest
                {
                    Content = "New cool"
                }));

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
        [Fact]
        public async Task Update_ReturnsUnAuthorized_WhenNotOwnCommentEvenWithAdmin()
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

            await AuthenticateAsAdminAsync();

            //Act
            var response = await TestClient.PatchAsync(ApiRoutes.Comment.Update.Replace("{id}", createdComment.Id.ToString()),
                JsonContent.Create(new UpdateCommentRequest
                {
                    Content = "New cool"
                }));

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Update_ReturnsNotFound_WhenCommentNotExists()
        {
            //Arrange
            await AuthenticateAsync();

            //Act
            var response = await TestClient.PatchAsync(ApiRoutes.Comment.Update.Replace("{id}", Guid.Empty.ToString()),
                JsonContent.Create(new UpdateCommentRequest
                {
                    Content = "Hello there"
                }));

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Update_ReturnsUpdatedComment_WhenOwnAndExists()
        {
            //Arrange
            var newContent = "Hello there";
            var createdPost = await CreatePostAsync(new CreatePostRequest
            {
                Content = "Hello",
                Title = "My first post",
                TagNames = new List<string>()
            });

            var createdComment = await CreateCommentAsync(createdPost.Id, new CreateCommentRequest
            {
                Content = "Hello"
            });

            await AuthenticateAsync();

            //Act
            var response = await TestClient.PatchAsync(ApiRoutes.Comment.Update.Replace("{id}", createdComment.Id.ToString()),
                JsonContent.Create(new UpdateCommentRequest
                {
                    Content = newContent
                }));

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var returnedComment = await response.Content.ReadAsAsync<Response<CommentResponse>>();
            returnedComment.Data.Content.Should().Be(newContent);
            returnedComment.Data.UpdatedAt.Day.Should().Be(DateTime.UtcNow.Day);

            var doubleCheck = await TestClient.GetAsync(ApiRoutes.Comment.Get.Replace("{id}", returnedComment.Data.Id.ToString()));
            doubleCheck.StatusCode.Should().Be(HttpStatusCode.OK);

            var doubleCheckComment = await doubleCheck.Content.ReadAsAsync<Response<CommentResponse>>();
            doubleCheckComment.Data.Content.Should().Be(newContent);
            doubleCheckComment.Data.UpdatedAt.Day.Should().Be(DateTime.UtcNow.Day);

            var trippleCheck = await TestClient.GetAsync(ApiRoutes.Post.Get.Replace("{id}", returnedComment.Data.PostId.ToString()));
            trippleCheck.StatusCode.Should().Be(HttpStatusCode.OK);

            var trippleCheckPost = await trippleCheck.Content.ReadAsAsync<Response<PostResponse>>();
            trippleCheckPost.Data.Comments.First(x => x.Id == returnedComment.Data.Id).Content.Should().Be(newContent);
            trippleCheckPost.Data.Comments.First(x => x.Id == returnedComment.Data.Id).UpdatedAt.Day.Should().Be(DateTime.UtcNow.Day);
        }

        [Fact]
        public async Task UpdateRating_ReturnsUnAuthorized_WhenNoAccount()
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
                Content = "Hello poster"
            });

            //Act
            var response = await TestClient.PatchAsync(ApiRoutes.Comment.UpdateRating.Replace("{id}", createdComment.Id.ToString()),
                new StringContent(string.Empty));

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task UpdateRating_ReturnsNotFound_WhenCommentNotExists()
        {
            //Arrange
            await AuthenticateAsync();

            //Act
            var response = await TestClient.PatchAsync(ApiRoutes.Comment.UpdateRating.Replace("{id}", Guid.Empty.ToString()),
                new StringContent(string.Empty));

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
        [Fact]
        public async Task UpdateRating_ReturnsUserAdded_WhenAccountAndNotLikedYet()
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
                Content = "Hey"
            });

            var token = await AuthenticateAsync();

            //Act
            var response = await TestClient.PatchAsync(ApiRoutes.Comment.UpdateRating.Replace("{id}", createdComment.Id.ToString()),
                new StringContent(string.Empty));

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var returnedProject = await response.Content.ReadAsAsync<Response<CommentResponse>>();
            returnedProject.Data.UserLikedIds.Should().Contain(GetUserId(token));
        }

        [Fact]
        public async Task UpdateRating_ReturnsUserRemoved_WhenAccountAndAlreadyLiked()
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
                Content = "Hey"
            });

            var token = await AuthenticateAsync();
            await TestClient.PatchAsync(ApiRoutes.Comment.UpdateRating.Replace("{id}", createdComment.Id.ToString()),
                new StringContent(string.Empty));

            //Act
            var response = await TestClient.PatchAsync(ApiRoutes.Comment.UpdateRating.Replace("{id}", createdComment.Id.ToString()),
                new StringContent(string.Empty));

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var returnedProject = await response.Content.ReadAsAsync<Response<CommentResponse>>();
            returnedProject.Data.UserLikedIds.Should().NotContain(GetUserId(token));
        }
    }
}