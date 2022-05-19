﻿using HelloWorldAPI;
using HelloWorldAPI.Contracts.V1;
using HelloWorldAPI.Contracts.V1.Requests;
using HelloWorldAPI.Contracts.V1.Responses;
using HelloWorldAPI.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace HelloWorld.IntegrationTests
{
    public class IntegrationTests
    {
        protected readonly HttpClient TestClient;

        private readonly string _dbName = Guid.NewGuid().ToString();

        protected IntegrationTests()
        {
            var appFactory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        services.RemoveAll(typeof(DbContextOptions<DataContext>));
                        services.AddDbContext<DataContext>(options =>
                        {
                            options.UseInMemoryDatabase(_dbName);
                        });
                    });
                });

            TestClient = appFactory.CreateClient();
        }

        protected void AuthenticateWithToken(string JwtToken)
        {
            TestClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", JwtToken);
        }


        protected async Task<string> AuthenticateAsync()
        {
            var token = await GetJwtAsync();
            TestClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return token;
        }

        protected async Task<string> AuthenticateAsAdminAsync()
        {
            var token = await GetAdminJwtAsync();
            TestClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return token;
        }

        protected async Task<string> GetJwtAsync()
        {
            var response = await TestClient.PostAsJsonAsync(ApiRoutes.Identity.Login, new UserLoginRequest
            {
                Email = "User@gmail.com",
                Password = "User1234!",
            });

            var registrationResponse = await response.Content.ReadAsAsync<AuthSuccessResponse>();
            return registrationResponse.Token;
        }

        protected async Task<string> GetAdminJwtAsync()
        {
            var response = await TestClient.PostAsJsonAsync(ApiRoutes.Identity.Login, new UserLoginRequest
            {
                Email = "Admin@gmail.com",
                Password = "Admin1234!",
            });

            var registrationResponse = await response.Content.ReadAsAsync<AuthSuccessResponse>();
            return registrationResponse.Token;
        }

        protected async Task<UserResponse> CreateUserAsync(CreateUserRequest request)
        {
            await AuthenticateAsAdminAsync();

            var response = await TestClient.PostAsJsonAsync(ApiRoutes.Identity.Create, request);
            var read = await response.Content.ReadAsAsync<Response<UserResponse>>();

            return read.Data;
        }

        protected async Task<string> RegisterUserAsync(UserRegistrationRequest request)
        {
            var response = await TestClient.PostAsJsonAsync(ApiRoutes.Identity.Register, request);
            var e = await response.Content.ReadAsStringAsync();

            var read = await response.Content.ReadAsAsync<AuthSuccessResponse>();
            return read.Token;
        }

        protected static string GetUserId(string jwtToken) => ExtractClaims(jwtToken).First(x => x.Type == "id").Value;

        private static IEnumerable<Claim> ExtractClaims(string jwtToken)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.ReadToken(jwtToken);
            if (IsJwtWithValidSecurityAlgorithm(securityToken, out var token))
            {
                var claims = token.Claims;
                return claims;
            }

            return null;
        }

        private static bool IsJwtWithValidSecurityAlgorithm(SecurityToken validatedToken, out JwtSecurityToken token)
        {
            if(validatedToken is JwtSecurityToken jwtSecurityToken &&
                jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                token = (JwtSecurityToken)validatedToken;
                return true;
            }

            token = null;
            return false;
        }
    }
}