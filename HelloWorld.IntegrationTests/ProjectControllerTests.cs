﻿using API.Contracts.V1;
using API.Contracts.V1.Requests;
using API.Contracts.V1.Responses;
using API.Domain.Filters;
using API.Extensions;
using FluentAssertions;
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
    public class ProjectControllerTests : IntegrationTests
    {
        [Fact]
        public async Task Create_ReturnUnAuthorized_WhenNoAccount()
        {
            //Act
            var response = await TestClient.PostAsJsonAsync(ApiRoutes.Project.Create, new CreateProjectRequest
            {
                Title = "HelloWorld",
                Description = "New Project here",
                MemberIds = new List<string>(),
                TagNames = new List<string>()
            });

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Create_ReturnCreatedProject_WhenAccountWithNewTagNames()
        {
            //Arrange
            var title = "HelloWorld";
            var description = "New project here";
            var tagNames = new List<string>() { "TagNames" };

            await AuthenticateAsync();

            //Act
            var response = await TestClient.PostAsJsonAsync(ApiRoutes.Project.Create, new CreateProjectRequest
            {
                Title = title,
                Description = description,
                MemberIds = new List<string>(),
                TagNames = tagNames,
            });

            var e = await response.Content.ReadAsStringAsync();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            var returnedProject = await response.Content.ReadAsAsync<Response<ProjectResponse>>();
            returnedProject.Data.Title.Should().Be(title);
            returnedProject.Data.Description.Should().Be(description);
            returnedProject.Data.Tags.Should().BeEquivalentTo(tagNames);
            returnedProject.Data.CreatedAt.Day.Should().Be(DateTime.UtcNow.Day);
            returnedProject.Data.UpdatedAt.Day.Should().Be(DateTime.UtcNow.Day);

            var doubleCheck = await TestClient.GetAsync(ApiRoutes.Project.Get.Replace("{id}", returnedProject.Data.Id.ToString()));
            doubleCheck.StatusCode.Should().Be(HttpStatusCode.OK);

            var doubleCheckProject = await doubleCheck.Content.ReadAsAsync<Response<ProjectResponse>>();
            doubleCheckProject.Data.Title.Should().Be(title);
            doubleCheckProject.Data.Description.Should().Be(description);
            doubleCheckProject.Data.Tags.Should().BeEquivalentTo(tagNames);
            doubleCheckProject.Data.CreatedAt.Day.Should().Be(DateTime.UtcNow.Day);
            doubleCheckProject.Data.UpdatedAt.Day.Should().Be(DateTime.UtcNow.Day);
        }

        [Fact]
        public async Task Create_ReturnCreatedProject_WhenAccountWithDuplicatedTagNames()
        {
            //Arrange
            var title = "HelloWorld";
            var description = "New project here";
            var tagNames = new List<string>() { "TagNames", "TagNames" };

            await AuthenticateAsync();

            //Act
            var response = await TestClient.PostAsJsonAsync(ApiRoutes.Project.Create, new CreateProjectRequest
            {
                Title = title,
                Description = description,
                MemberIds = new List<string>(),
                TagNames = tagNames,
            });

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            var returnedProject = await response.Content.ReadAsAsync<Response<ProjectResponse>>();
            returnedProject.Data.Title.Should().Be(title);
            returnedProject.Data.Description.Should().Be(description);
            returnedProject.Data.Tags.Should().BeEquivalentTo(tagNames.Distinct());
            returnedProject.Data.CreatedAt.Day.Should().Be(DateTime.UtcNow.Day);
            returnedProject.Data.UpdatedAt.Day.Should().Be(DateTime.UtcNow.Day);

            var doubleCheck = await TestClient.GetAsync(ApiRoutes.Project.Get.Replace("{id}", returnedProject.Data.Id.ToString()));
            doubleCheck.StatusCode.Should().Be(HttpStatusCode.OK);

            var doubleCheckProject = await doubleCheck.Content.ReadAsAsync<Response<ProjectResponse>>();
            doubleCheckProject.Data.Title.Should().Be(title);
            doubleCheckProject.Data.Description.Should().Be(description);
            doubleCheckProject.Data.Tags.Should().BeEquivalentTo(tagNames.Distinct());
            doubleCheckProject.Data.CreatedAt.Day.Should().Be(DateTime.UtcNow.Day);
            doubleCheckProject.Data.UpdatedAt.Day.Should().Be(DateTime.UtcNow.Day);
        }

        [Fact]
        public async Task Create_ReturnCreatedProject_WhenAccountWithMixedTagNames()
        {
            //Arrange

            var title = "HelloWorld";
            var description = "New project here";
            var tagNames = new List<string>() { "TagNames", "New Tag" };

            var createedDiscussion = await CreateProjectAsnyc(new CreateProjectRequest
            {
                Title = "HelloWorld2",
                Description = "Another new project",
                MemberIds = new List<string>(),
                TagNames = new List<string>() { "TagNames" }
            });

            await AuthenticateAsync();

            //Act
            var response = await TestClient.PostAsJsonAsync(ApiRoutes.Project.Create, new CreateProjectRequest
            {
                Title = title,
                Description = description,
                MemberIds = new List<string>(),
                TagNames = tagNames,
            });

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            var returnedProject = await response.Content.ReadAsAsync<Response<ProjectResponse>>();
            returnedProject.Data.Title.Should().Be(title);
            returnedProject.Data.Description.Should().Be(description);
            returnedProject.Data.Tags.Should().BeEquivalentTo(tagNames.Distinct());
            returnedProject.Data.CreatedAt.Day.Should().Be(DateTime.UtcNow.Day);
            returnedProject.Data.UpdatedAt.Day.Should().Be(DateTime.UtcNow.Day);

            var doubleCheck = await TestClient.GetAsync(ApiRoutes.Project.Get.Replace("{id}", returnedProject.Data.Id.ToString()));
            doubleCheck.StatusCode.Should().Be(HttpStatusCode.OK);

            var doubleCheckProject = await doubleCheck.Content.ReadAsAsync<Response<ProjectResponse>>();
            doubleCheckProject.Data.Title.Should().Be(title);
            doubleCheckProject.Data.Description.Should().Be(description);
            doubleCheckProject.Data.Tags.Should().BeEquivalentTo(tagNames.Distinct());
            doubleCheckProject.Data.CreatedAt.Day.Should().Be(DateTime.UtcNow.Day);
            doubleCheckProject.Data.UpdatedAt.Day.Should().Be(DateTime.UtcNow.Day);
        }

        [Fact]
        public async Task Delete_ReturnsUnAuthorized_WhenNoAccount()
        {
            //Arrange
            var createdProject = await CreateProjectAsnyc(new CreateProjectRequest
            {
                Title = "HelloWorld",
                Description = "New project here",
                MemberIds = new List<string>(),
                TagNames = new List<string>()
            });

            //Act
            var response = await TestClient.DeleteAsync(ApiRoutes.Project.Delete.Replace("{id}", createdProject.Id.ToString()));

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Delete_ReturnsUnAuthorized_WhenNotOwnProject()
        {
            //Arrange
            var createdProject = await CreateProjectAsnyc(new CreateProjectRequest
            {
                Title = "HelloWorld",
                Description = "New project here",
                MemberIds = new List<string>(),
                TagNames = new List<string>()
            });

            await AuthenticateAsSecondAsync();

            //Act
            var response = await TestClient.DeleteAsync(ApiRoutes.Project.Delete.Replace("{id}", createdProject.Id.ToString()));

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Delete_ReturnsNoContent_WhenOwnAccount()
        {
            //Arrange
            var createdProject = await CreateProjectAsnyc(new CreateProjectRequest
            {
                Title = "HelloWorld",
                Description = "New project here",
                MemberIds = new List<string>(),
                TagNames = new List<string>()
            });

            await AuthenticateAsync();

            //Act
            var response = await TestClient.DeleteAsync(ApiRoutes.Project.Delete.Replace("{id}", createdProject.Id.ToString()));

            //Assert 
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);

            var doubleCheck = await TestClient.GetAsync(ApiRoutes.Project.Get.Replace("{id}", createdProject.Id.ToString()));
            doubleCheck.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Delete_ReturnsNoContent_WhenAdminAccount()
        {
            //Arrange
            var createdProject = await CreateProjectAsnyc(new CreateProjectRequest
            {
                Title = "HelloWorld",
                Description = "New project here",
                MemberIds = new List<string>(),
                TagNames = new List<string>()
            });

            await AuthenticateAsAdminAsync();

            //Act
            var response = await TestClient.DeleteAsync(ApiRoutes.Project.Delete.Replace("{id}", createdProject.Id.ToString()));

            //Assert 
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);

            var doubleCheck = await TestClient.GetAsync(ApiRoutes.Project.Get.Replace("{id}", createdProject.Id.ToString()));
            doubleCheck.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_WhenProjectNotExists()
        {
            //Arrange
            await AuthenticateAsync();

            //Act
            var response = await TestClient.DeleteAsync(ApiRoutes.Project.Delete.Replace("{id}", Guid.Empty.ToString()));

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Get_ReturnsUnAuthorized_WhenNoAccount()
        {
            //Arrange
            var createdProject = await CreateProjectAsnyc(new CreateProjectRequest
            {
                Title = "HelloWorld",
                Description = "New Project here",
                MemberIds = new List<string>(),
                TagNames = new List<string>()
            });

            //Act
            var response = await TestClient.GetAsync(ApiRoutes.Project.Get.Replace("{id}", createdProject.Id.ToString()));

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Get_ReturnsNotFound_WhenProjectDontExists()
        {
            //Assert
            await AuthenticateAsync();

            //Act
            var response = await TestClient.GetAsync(ApiRoutes.Project.Get.Replace("{id}", Guid.Empty.ToString()));

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Get_ReturnsProject_WhenAccountAndProjectExists()
        {
            var title = "HelloWorld";
            var description = "New project here";

            //Assert
            var createdProject = await CreateProjectAsnyc(new CreateProjectRequest
            {
                Title = title,
                Description = description,
                MemberIds = new List<string>(),
                TagNames = new List<string>()
            });

            await AuthenticateAsync();

            //Act
            var response = await TestClient.GetAsync(ApiRoutes.Project.Get.Replace("{id}", createdProject.Id.ToString()));

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var returnedProject = await response.Content.ReadAsAsync<Response<ProjectResponse>>();
            returnedProject.Data.Title.Should().Be(title);
            returnedProject.Data.Description.Should().Be(description);
        }

        [Fact]
        public async Task GetAll_ReturnsUnAuthorized_WhenNoAccount()
        {
            //Act
            var response = await TestClient.GetAsync(ApiRoutes.Project.GetAll);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task GetAll_ReturnsZeroData_WhenFreshDatabase()
        {
            //Arrange
            await AuthenticateAsync();

            //Act
            var response = await TestClient.GetAsync(ApiRoutes.Project.GetAll);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var returnedDiscussions = await response.Content.ReadAsAsync<PagedResponse<PartialProjectResponse>>();
            returnedDiscussions.Data.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAll_ReturnsProjectOne_WhenApplyFilter()
        {
            //Arrange
            var title = "HelloWorld";
            var description = "New project here";
            var tagNames = new List<string>() { "Tag" };

            //Assert
            var createdProjectOne = await CreateProjectAsnyc(new CreateProjectRequest
            {
                Title = title,
                Description = description,
                MemberIds = new List<string>(),
                TagNames = tagNames
            });

            await CreateProjectAsnyc(new CreateProjectRequest
            {
                Title = "HelloWorld2",
                Description = "New project here",
                MemberIds = new List<string>(),
                TagNames = new List<string>()
            });

            var filter = new GetAllProjectsFilter
            {
                Title = "Hello",
                Tags = tagNames
            };

            await AuthenticateAsync();

            //Act
            var response = await TestClient.GetAsync(ApiRoutes.Project.GetAll + filter.ToQueryString());

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var returnedProjects = await response.Content.ReadAsAsync<PagedResponse<PartialProjectResponse>>();
            returnedProjects.Data.Should().HaveCount(1);
            returnedProjects.Data.First().Title.Should().Be(title);
            returnedProjects.Data.First().Description.Should().Be(description);
            tagNames.Should().BeSubsetOf(returnedProjects.Data.First().Tags);
        }

        [Fact]
        public async Task GetAll_ReturnsCorrectPagination_WhenEmptyData()
        {
            //Arrange
            await AuthenticateAsync();

            //Act
            var response = await TestClient.GetAsync(ApiRoutes.Project.GetAll);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var returnedComments = await response.Content.ReadAsAsync<PagedResponse<PartialProjectResponse>>();
            returnedComments.NextPage.Should().BeNull();
            returnedComments.PreviousPage.Should().BeNull();
            returnedComments.PageNumber.Should().Be(1);
        }

        [Fact]
        public async Task GetAll_ReturnsCorrectPagination_WhenHaveData()
        {
            //Arrange

            for (int i = 0; i < 5; i++)
            {
                await CreateProjectAsnyc(new CreateProjectRequest
                {
                    Title = "HelloWorld2",
                    Description = "New project here",
                    MemberIds = new List<string>(),
                    TagNames = new List<string>()
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
            var response = await TestClient.GetAsync(ApiRoutes.Project.GetAll + paginationFilter.ToQueryString());

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var returnedComments = await response.Content.ReadAsAsync<PagedResponse<PartialProjectResponse>>();
            returnedComments.NextPage.Should().Be(GetAllUriNext(ApiRoutes.Project.GetAll, pageNumber, pageSize));
            returnedComments.PreviousPage.Should().Be(GetAllUriLast(ApiRoutes.Project.GetAll, pageNumber, pageSize));
            returnedComments.PageNumber.Should().Be(pageNumber);
        }

        [Fact]
        public async Task Update_ReturnsNoAuthorized_WhenNoAccount()
        {
            //Arrange
            var createdProject = await CreateProjectAsnyc(new CreateProjectRequest
            {
                Title = "HelloWorld",
                Description = "New Project here",
                MemberIds = new List<string>(),
                TagNames = new List<string>()
            });

            //Act
            var response = await TestClient.PatchAsync(ApiRoutes.Project.Update.Replace("{id}", createdProject.Id.ToString()),
                JsonContent.Create(new UpdateProjectRequest
                {
                    Description = "Test",
                    MemberIds = new List<string>(),
                    TagNames = new List<string>(),
                    Title = "Test"
                }));

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Update_ReturnsNoAuthorized_WhenNotOwnProject()
        {
            //Arrange
            var createdProject = await CreateProjectAsnyc(new CreateProjectRequest
            {
                Title = "HelloWorld",
                Description = "New Project here",
                MemberIds = new List<string>(),
                TagNames = new List<string>()
            });

            await AuthenticateAsSecondAsync();

            //Act
            var response = await TestClient.PatchAsync(ApiRoutes.Project.Update.Replace("{id}", createdProject.Id.ToString()),
                JsonContent.Create(new UpdateProjectRequest
                {
                    Description = "Test",
                    MemberIds = new List<string>(),
                    TagNames = new List<string>(),
                    Title = "Test"
                }));

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Update_ReturnsNoAuthorized_WhenNotOwnProjectEvenWithAdmin()
        {
            //Arrange
            var createdProject = await CreateProjectAsnyc(new CreateProjectRequest
            {
                Title = "HelloWorld",
                Description = "New project here",
                MemberIds = new List<string>(),
                TagNames = new List<string>()
            });

            await AuthenticateAsAdminAsync();

            //Act
            var response = await TestClient.PatchAsync(ApiRoutes.Project.Update.Replace("{id}", createdProject.Id.ToString()),
                JsonContent.Create(new UpdateProjectRequest
                {
                    Description = "Test",
                    MemberIds = new List<string>(),
                    TagNames = new List<string>(),
                    Title = "Test"
                }));

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Update_ReturnsNotFound_WhenProjectNotExists()
        {
            //Arrange
            await AuthenticateAsync();

            //Act
            var response = await TestClient.PatchAsync(ApiRoutes.Project.Update.Replace("{id}", Guid.Empty.ToString()),
                JsonContent.Create(new UpdateProjectRequest
                {
                    Description = "Test",
                    MemberIds = new List<string>(),
                    TagNames = new List<string>(),
                    Title = "Test"
                }));

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Update_ReturnsUpdatedProject_WhenOwnAndProjectExistsWithNewTagNames()
        {
            //Arrange
            var token = await AuthenticateAsSecondAsync();

            var newTitle = "Updated HelloWorld";
            var newDescription = "Update project here";
            var newMembers = new List<string>() { GetUserId(token) };
            var newTagNames = new List<string>() { "Tag" };

            var createdProject = await CreateProjectAsnyc(new CreateProjectRequest
            {
                Title = "HelloWorld",
                Description = "New Project here",
                MemberIds = new List<string>(),
                TagNames = new List<string>()
            });

            await AuthenticateAsync();

            //Act
            var response = await TestClient.PatchAsync(ApiRoutes.Project.Update.Replace("{id}", createdProject.Id.ToString()),
                JsonContent.Create(new UpdateProjectRequest
                {
                    MemberIds = newMembers,
                    Description = newDescription,
                    TagNames = newTagNames,
                    Title = newTitle
                }));

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var returnedProject = await response.Content.ReadAsAsync<Response<ProjectResponse>>();
            returnedProject.Data.Title.Should().Be(newTitle);
            returnedProject.Data.Description.Should().Be(newDescription);
            returnedProject.Data.UpdatedAt.Day.Should().Be(DateTime.UtcNow.Day);
            returnedProject.Data.Tags.Should().BeEquivalentTo(newTagNames);

            var doubleCheck = await TestClient.GetAsync(ApiRoutes.Project.Get.Replace("{id}", createdProject.Id.ToString()));
            doubleCheck.StatusCode.Should().Be(HttpStatusCode.OK);

            var doubleCheckProject = await doubleCheck.Content.ReadAsAsync<Response<ProjectResponse>>();
            doubleCheckProject.Data.Title.Should().Be(newTitle);
            doubleCheckProject.Data.Description.Should().Be(newDescription);
            doubleCheckProject.Data.UpdatedAt.Day.Should().Be(DateTime.UtcNow.Day);
            doubleCheckProject.Data.Tags.Should().BeEquivalentTo(newTagNames);
        }

        [Fact]
        public async Task Update_ReturnsUpdatedProject_WhenOwnAndProjectExistsWithWithDuplicateTagNames()
        {
            //Arrange
            var token = await AuthenticateAsSecondAsync();

            var newTitle = "Updated HelloWorld";
            var newDescription = "Update project here";
            var newMembers = new List<string>() { GetUserId(token) };
            var newTagNames = new List<string>() { "Tag", "Tag" };

            var createdProject = await CreateProjectAsnyc(new CreateProjectRequest
            {
                Title = "HelloWorld",
                Description = "New Project here",
                MemberIds = new List<string>(),
                TagNames = new List<string>()
            });

            await AuthenticateAsync();

            //Act
            var response = await TestClient.PatchAsync(ApiRoutes.Project.Update.Replace("{id}", createdProject.Id.ToString()),
                JsonContent.Create(new UpdateProjectRequest
                {
                    MemberIds = newMembers,
                    Description = newDescription,
                    TagNames = newTagNames,
                    Title = newTitle
                }));

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var returnedProject = await response.Content.ReadAsAsync<Response<ProjectResponse>>();
            returnedProject.Data.Title.Should().Be(newTitle);
            returnedProject.Data.Description.Should().Be(newDescription);
            returnedProject.Data.UpdatedAt.Day.Should().Be(DateTime.UtcNow.Day);
            returnedProject.Data.Tags.Should().BeEquivalentTo(newTagNames.Distinct());

            var doubleCheck = await TestClient.GetAsync(ApiRoutes.Project.Get.Replace("{id}", createdProject.Id.ToString()));
            doubleCheck.StatusCode.Should().Be(HttpStatusCode.OK);

            var doubleCheckProject = await doubleCheck.Content.ReadAsAsync<Response<ProjectResponse>>();
            doubleCheckProject.Data.Title.Should().Be(newTitle);
            doubleCheckProject.Data.Description.Should().Be(newDescription);
            doubleCheckProject.Data.UpdatedAt.Day.Should().Be(DateTime.UtcNow.Day);
            doubleCheckProject.Data.Tags.Should().BeEquivalentTo(newTagNames.Distinct());
        }

        [Fact]
        public async Task Update_ReturnsUpdatedProject_WhenOwnAndProjectExistsWithWithMixedTagNames()
        {
            //Arrange
            var token = await AuthenticateAsSecondAsync();

            var newTitle = "Updated HelloWorld";
            var newDescription = "Update project here";
            var newMembers = new List<string>() { GetUserId(token) };
            var newTagNames = new List<string>() { "Tag", "new Tag" };

            var createdProject = await CreateProjectAsnyc(new CreateProjectRequest
            {
                Title = "HelloWorld",
                Description = "New Project here",
                MemberIds = new List<string>(),
                TagNames = new List<string>()
            });

            await CreateProjectAsnyc(new CreateProjectRequest
            {
                Title = "HelloWorld",
                Description = "New Project here",
                MemberIds = new List<string>(),
                TagNames = new List<string>() { "Tag" }
            });

            await AuthenticateAsync();

            //Act
            var response = await TestClient.PatchAsync(ApiRoutes.Project.Update.Replace("{id}", createdProject.Id.ToString()),
                JsonContent.Create(new UpdateProjectRequest
                {
                    MemberIds = newMembers,
                    Description = newDescription,
                    TagNames = newTagNames,
                    Title = newTitle
                }));

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var returnedProject = await response.Content.ReadAsAsync<Response<ProjectResponse>>();
            returnedProject.Data.Title.Should().Be(newTitle);
            returnedProject.Data.Description.Should().Be(newDescription);
            returnedProject.Data.UpdatedAt.Day.Should().Be(DateTime.UtcNow.Day);
            returnedProject.Data.Tags.Should().BeEquivalentTo(newTagNames);

            var doubleCheck = await TestClient.GetAsync(ApiRoutes.Project.Get.Replace("{id}", createdProject.Id.ToString()));
            doubleCheck.StatusCode.Should().Be(HttpStatusCode.OK);

            var doubleCheckProject = await doubleCheck.Content.ReadAsAsync<Response<ProjectResponse>>();
            doubleCheckProject.Data.Title.Should().Be(newTitle);
            doubleCheckProject.Data.Description.Should().Be(newDescription);
            doubleCheckProject.Data.UpdatedAt.Day.Should().Be(DateTime.UtcNow.Day);
            doubleCheckProject.Data.Tags.Should().BeEquivalentTo(newTagNames);
        }

        [Fact]
        public async Task UpdateRating_ReturnsUnAuthorized_WhenNoAccount()
        {
            //Arrange
            var createdProject = await CreateProjectAsnyc(new CreateProjectRequest
            {
                Title = "HelloWorld",
                Description = "New Project here",
                MemberIds = new List<string>(),
                TagNames = new List<string>() { "Tag" }
            });

            //Act
            var response = await TestClient.PatchAsync(ApiRoutes.Project.UpdateRating.Replace("{id}", createdProject.Id.ToString()),
                new StringContent(string.Empty));

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task UpdateRating_ReturnsNotFound_WhenProjectNotExists()
        {
            //Arrange
            await AuthenticateAsync();

            //Act
            var response = await TestClient.PatchAsync(ApiRoutes.Project.UpdateRating.Replace("{id}", Guid.Empty.ToString()),
                new StringContent(string.Empty));

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task UpdateRating_ReturnsUserAdded_WhenAccountAndNotLikedYet()
        {
            //Arrange
            var createdProject = await CreateProjectAsnyc(new CreateProjectRequest
            {
                Title = "HelloWorld",
                Description = "New Project here",
                MemberIds = new List<string>(),
                TagNames = new List<string>() { "Tag" }
            });

            var token = await AuthenticateAsync();

            //Act
            var response = await TestClient.PatchAsync(ApiRoutes.Project.UpdateRating.Replace("{id}", createdProject.Id.ToString()),
                new StringContent(string.Empty));

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var returnedProject = await response.Content.ReadAsAsync<Response<ProjectResponse>>();
            returnedProject.Data.UsersLikedIds.Should().Contain(GetUserId(token));
        }

        [Fact]
        public async Task UpdateRating_ReturnsUserRemoved_WhenAccountAndAlreadyLiked()
        {
            //Arrange
            var token = await AuthenticateAsync();

            var createdProject = await CreateProjectAsnyc(new CreateProjectRequest
            {
                Title = "HelloWorld",
                Description = "New Project here",
                MemberIds = new List<string>() { GetUserId(token) },
                TagNames = new List<string>() { "Tag" }
            });

            await AuthenticateAsync();
            await TestClient.PatchAsync(ApiRoutes.Project.UpdateRating.Replace("{id}", createdProject.Id.ToString()), new StringContent(string.Empty));

            //Act
            var response = await TestClient.PatchAsync(ApiRoutes.Project.UpdateRating.Replace("{id}", createdProject.Id.ToString()),
                new StringContent(string.Empty));

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var returnedProject = await response.Content.ReadAsAsync<Response<ProjectResponse>>();
            returnedProject.Data.UsersLikedIds.Should().NotContain(GetUserId(token));
        }
    }
}
