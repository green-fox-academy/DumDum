using DumDum;
using DumDum.Models.JsonEntities;
using DumDum.Models.JsonEntities.Authorization;
using DumDum.Models.JsonEntities.Login;
using DumDum.Models.JsonEntities.Player;
using DumDum.Models.JsonEntities.Troops;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using Xunit;

namespace IntegrationTests
{
    public class TroopControllerTests : TestService, IClassFixture<WebApplicationFactory<Startup>>
    {
        [Fact]
        public void KingdomsLeaderboard_ShouldReturOKAndLeaderboardList_WhenRequestIsCorrect()
        {
            //arrange
            HttpStatusCode expectedStatusCode = HttpStatusCode.OK;
            var request = new HttpRequestMessage();
            request.RequestUri = new Uri("https://localhost:20625/leaderboards/kingdoms");
            request.Method = HttpMethod.Get;

            //act
            HttpResponseMessage response = HttpClient.SendAsync(request).Result;

            //assert
            Assert.Equal(expectedStatusCode, response.StatusCode);
        }

        [Fact]
        public void Troops_ShouldReturnOk_WhenRequestCorrect()
        {
            var request = new HttpRequestMessage();
            var tokenResult = TestLoginReturnToken("Nya", "catcatcat");

            var inputObj = JsonConvert.SerializeObject(new { kingdomId = 1 });
            StringContent requestContent = new(inputObj, Encoding.UTF8, "application/json");
            request.RequestUri = new Uri("http://localhost:20625/kingdoms/1/troops");
            request.Method = HttpMethod.Get;
            request.Content = requestContent;
            request.Headers.Add("authorization", $"bearer {tokenResult}");
            var response = HttpClient.SendAsync(request).Result;

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public void Troops_ShouldReturnUnauthorized_WhenNotCorrectPlayerLoggedIn()
        {
            var request = new HttpRequestMessage();
            var tokenResult = TestLoginReturnToken("Nya", "catcatcat");

            var inputObj = JsonConvert.SerializeObject(new { kingdomId = 1 });
            StringContent requestContent = new(inputObj, Encoding.UTF8, "application/json");
            request.RequestUri = new Uri("http://localhost:20625/kingdoms/7/troops");
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
        public void TroopsLeaderboard_ShouldReturnStatusOk_WhenRequestDoneCorrectly()
        {
            //arrange
            HttpStatusCode expectedStatusCode = HttpStatusCode.OK;
            var request = new HttpRequestMessage();
            request.RequestUri = new Uri("https://localhost:20625/leaderboards/troops");
            request.Method = HttpMethod.Get;

            //act
            HttpResponseMessage response = HttpClient.SendAsync(request).Result;

            //assert
            Assert.Equal(expectedStatusCode, response.StatusCode);
        }
        [Fact]
        public void HttpPutUpgradeTroops_ReturnsUnauhtorizedAndError()
        {
            //arrange
            var request = new HttpRequestMessage();
            var tokenResult = TestLoginReturnToken("Nya", "catcatcat");
            ErrorResponse expectedErrorResult = new();
            expectedErrorResult.Error = "This kingdom does not belong to authenticated player";
            HttpStatusCode expectedStatusCode = HttpStatusCode.Unauthorized;

            TroopUpgradeRequest requestBody = new();
            requestBody.Type = "Phalanx";
            string requestBodyContent = JsonConvert.SerializeObject(requestBody);
            StringContent requestContent = new(requestBodyContent, Encoding.UTF8, "application/json");
            request.RequestUri = new Uri("http://localhost:20625/kingdoms/2/troops");
            request.Method = HttpMethod.Put;
            request.Content = requestContent;
            request.Headers.Add("authorization", $"bearer {tokenResult}");

            //act
            var response = HttpClient.SendAsync(request).Result;
            string responseBodyContent = response.Content.ReadAsStringAsync().Result;
            ErrorResponse responseData = JsonConvert.DeserializeObject<ErrorResponse>(responseBodyContent);

            //assert
            Assert.Equal(expectedStatusCode, response.StatusCode);
            Assert.Equal(expectedErrorResult.Error, responseData.Error);
        }

        [Fact]
        public void UpgradeTroops_ReturnsBadRequestAndError()
        {
            //arrange
            var request = new HttpRequestMessage();
            var tokenResult = TestLoginReturnToken("Nya", "catcatcat");
            ErrorResponse expectedErrorResult = new();
            expectedErrorResult.Error = "Request was not done correctly!";
            HttpStatusCode expectedStatusCode = HttpStatusCode.BadRequest;

            TroopUpgradeRequest requestBody = new();
            requestBody.Type = "phalanxios";
            string requestBodyContent = JsonConvert.SerializeObject(requestBody);
            StringContent requestContent = new(requestBodyContent, Encoding.UTF8, "application/json");
            request.RequestUri = new Uri("http://localhost:20625/kingdoms/1/troops");
            request.Method = HttpMethod.Put;
            request.Content = requestContent;
            request.Headers.Add("authorization", $"bearer {tokenResult}");

            //act
            var response = HttpClient.SendAsync(request).Result;
            string responseBodyContent = response.Content.ReadAsStringAsync().Result;
            ErrorResponse responseData = JsonConvert.DeserializeObject<ErrorResponse>(responseBodyContent);

            //assert
            Assert.Equal(expectedStatusCode, response.StatusCode);
            Assert.Equal(expectedErrorResult.Error, responseData.Error);
        }
    }
}