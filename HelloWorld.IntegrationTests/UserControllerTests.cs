using FluentAssertions;
using HelloWorldAPI.Contracts.V1;
using HelloWorldAPI.Contracts.V1.Responses;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace HelloWorld.IntegrationTests
{
    public class UserControllerTests : IntegrationTests
    {
        [Fact]
        public async Task GetAll_WithOneUser_ReturnsOneUser()
        {
            //Arrange
            await AuthenticateAsync();

            //Act
            var response = await TestClient.GetAsync(ApiRoutes.Identity.GetAll);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            (await response.Content.ReadAsAsync<Response<List<UserResponse>>>()).Data.Should().HaveCount(4);
        }

       
    }
}
