using DumDum;
using DumDum.Database;
using DumDum.Models.JsonEntities;
using DumDum.Models.JsonEntities.Kingdom;
using DumDum.Models.JsonEntities.Login;
using DumDum.Models.JsonEntities.Player;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace TestProjectMock
{
    public class TestService
    {
        private HttpClient HttpClient { get; set; }
        public TestService()
        {
            var appFactory = new WebApplicationFactory<Startup>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        services.RemoveAll(typeof(ApplicationDbContext));
                        services.AddDbContext<ApplicationDbContext>(options => { options.UseInMemoryDatabase("TestDB"); });
                    });
                });
            HttpClient = appFactory.CreateClient();
        }

        public void TestLoginReturnToken()
        {
            HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", GetJwt());
        }

        public string GetJwt()
        {
            var inputObj = JsonConvert.SerializeObject(new PlayerRequest() { Username = "TestUser", Password = "TestPwd", KingdomName="TestKingdom" });
            StringContent requestContent = new(inputObj, Encoding.UTF8, "application/json");
            var response = HttpClient.PostAsync("https://localhost:20625/registration", requestContent).Result;
            string contentResponse = response.Content.ReadAsStringAsync().Result;
            LoginResponse token = JsonConvert.DeserializeObject<LoginResponse>(contentResponse);
            return token.Token;
        }

        [Fact]
        public void HttpPutRegistration_ReturnsBadRequestAndErrorResponse()
        {
            //arrange
            var request = new HttpRequestMessage();
            TestLoginReturnToken();
            ErrorResponse expectedError = new();
            expectedError.Error = "One or both coordinates are out of valid range(0 - 99).";
            HttpStatusCode expectedStatusCode = HttpStatusCode.BadRequest;

            KingdomRegistrationRequest requestBody = new();
            requestBody.CoordinateY = 100;
            requestBody.CoordinateX = 88;
            requestBody.KingdomId = 1;
            string requestBodyContent = JsonConvert.SerializeObject(requestBody);
            StringContent requestContent = new(requestBodyContent, Encoding.UTF8, "application/json");
            request.RequestUri = new Uri("http://localhost:5000/registration");
            request.Method = HttpMethod.Put;
            request.Content = requestContent;
            //request.Headers.Add("authorization", $"bearer {tokenResult}");

            //act
            var response = HttpClient.SendAsync(request).Result;
            string responseBodyContent = response.Content.ReadAsStringAsync().Result;
            ErrorResponse responseData = JsonConvert.DeserializeObject<ErrorResponse>(responseBodyContent);

            //assert
            Assert.Equal(expectedStatusCode, response.StatusCode);
            Assert.Equal(expectedError.Error, responseData.Error);
        }
    }
}
