using DumDum;
using DumDum.Models.JsonEntities;
using DumDum.Models.JsonEntities.Buildings;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using Xunit;

namespace IntegrationTests
{
    public class BuildingControllerTests : TestService, IClassFixture<WebApplicationFactory<Startup>>
    {
        
        [Fact]
        public void Buildings_ShouldReturnOK_WhenPlayerExist()
        {
            var request = new HttpRequestMessage();
            var tokenResult = TestLoginReturnToken("Nya", "catcatcat");

            var inputObj = JsonConvert.SerializeObject(new BuildingRequest() { Id = 1 }); //vstupovat s parametry, jako controller
            StringContent requestContent = new(inputObj, Encoding.UTF8, "application/json");
            request.RequestUri = new Uri("http://localhost:20625/kingdoms/1/buildings");
            request.Method = HttpMethod.Get;
            request.Content = requestContent;
            request.Headers.Add("authorization", $"bearer {tokenResult}");
            var response = HttpClient.SendAsync(request).Result;

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public void Buildings_ShouldReturnUnathorized_WhenIncorrectPlayerLoggedIn()
        {
            var request = new HttpRequestMessage();
            var tokenResult = TestLoginReturnToken("Nya", "catcatcat");

            var inputObj = JsonConvert.SerializeObject(new BuildingRequest() { Id = 1 }); //vstupovat s parametry, jako controller
            StringContent requestContent = new(inputObj, Encoding.UTF8, "application/json");
            request.RequestUri = new Uri("http://localhost:20625/kingdoms/2/buildings");
            request.Method = HttpMethod.Get;
            request.Content = requestContent;
            request.Headers.Add("authorization", $"bearer {tokenResult}");
            var response = HttpClient.SendAsync(request).Result;
            string respond = response.Content.ReadAsStringAsync().Result;
            ErrorResponse error = JsonConvert.DeserializeObject<ErrorResponse>(respond);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.Equal("This kingdom does not belong to authenticated player", error.Error);
        }

        [Fact]
        public void AddBuilding_ShouldReturnOk_WhenRequestIsCorrect()
        {
            var request = new HttpRequestMessage();
            var tokenResult = TestLoginReturnToken("Nya", "catcatcat");

            var inputObj = JsonConvert.SerializeObject(new BuildingAddRequest() { Type = "farm" });
            StringContent requestContent = new(inputObj, Encoding.UTF8, "application/json");
            request.RequestUri = new Uri("https://localhost:20625/kingdoms/1/buildings");
            request.Method = HttpMethod.Post;
            request.Content = requestContent;
            request.Headers.Add("authorization", $"bearer {tokenResult}");
            var response = HttpClient.SendAsync(request).Result;

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public void AddBuilding_ShouldReturnNotAcceptable_WhenWrongTypeProvided()
        {
            var request = new HttpRequestMessage();
            var tokenResult = TestLoginReturnToken("Nya", "catcatcat");

            var inputObj = JsonConvert.SerializeObject(new BuildingAddRequest() { Type = "arm" });
            StringContent requestContent = new(inputObj, Encoding.UTF8, "application/json");
            request.RequestUri = new Uri("https://localhost:20625/kingdoms/1/buildings/");
            request.Method = HttpMethod.Post;
            request.Content = requestContent;
            request.Headers.Add("authorization", $"bearer {tokenResult}");
            var response = HttpClient.SendAsync(request).Result;

            Assert.Equal(HttpStatusCode.NotAcceptable, response.StatusCode);
        }

        [Fact]
        public void AddBuilding_ShouldReturnUnathorized_WhenIncorrectPlayerLoggedIn()
        {
            var request = new HttpRequestMessage();

            var inputObj = JsonConvert.SerializeObject(new BuildingAddRequest() { Type = "farm" });
            StringContent requestContent = new(inputObj, Encoding.UTF8, "application/json");
            request.RequestUri = new Uri("https://localhost:20625/kingdoms/1/buildings/");
            request.Method = HttpMethod.Post;
            request.Content = requestContent;
            var response = HttpClient.SendAsync(request).Result;

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public void BuildingLeaderboard_ShouldReturOKAndLeaderboardList_WhenRequestIsCorrect()
        {
            //arrange
            HttpStatusCode expectedStatusCode = HttpStatusCode.OK;

            var request = new HttpRequestMessage();
            request.RequestUri = new Uri("https://localhost:20625/leaderboards/buildings");
            request.Method = HttpMethod.Get;

            //act
            HttpResponseMessage response = HttpClient.SendAsync(request).Result;

            //assert
            Assert.Equal(expectedStatusCode, response.StatusCode);
        }

        [Fact]
        public void UpgradeBuildings_ShouldReturnBadRequest_WhenRequestNotCorrect()
        {
            var request = new HttpRequestMessage();
            var tokenResult = TestLoginReturnToken("nya", "catcatcat");

            var inputObj = JsonConvert.SerializeObject(new UpgradeBuildingRequest() { KingdomId = 1 });
            StringContent requestContent = new(inputObj, Encoding.UTF8, "application/json");
            request.RequestUri = new Uri("http://localhost:20625/kingdoms/1/buildings/");
            request.Method = HttpMethod.Put;
            request.Content = requestContent;
            request.Headers.Add("authorization", $"bearer {tokenResult}");
            var response = HttpClient.SendAsync(request).Result;

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public void UpgradeBuildings_ShouldReturnUnauthorized_WhenWrongPasswordProvided()
        {
            var request = new HttpRequestMessage();
            var tokenResult = TestLoginReturnToken("Nya", "cattcat");

            var inputObj = JsonConvert.SerializeObject(new UpgradeBuildingRequest() { KingdomId = 1, BuildingId = 1 });
            StringContent requestContent = new(inputObj, Encoding.UTF8, "application/json");
            request.RequestUri = new Uri("http://localhost:20625/kingdoms/1/buildings/1");
            request.Method = HttpMethod.Put;
            request.Content = requestContent;
            request.Headers.Add("authorization", $"bearer {tokenResult}");
            var response = HttpClient.SendAsync(request).Result;

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}
