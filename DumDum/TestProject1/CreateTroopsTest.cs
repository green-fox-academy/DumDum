using System;
using System.Net;
using System.Net.Http;
using System.Text;
using DumDum;
using DumDum.Models.JsonEntities;
using DumDum.Models.JsonEntities.Login;
using DumDum.Models.JsonEntities.Player;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;

namespace TestProject1
{
    public class CreateTroopsTest : IClassFixture<WebApplicationFactory<Startup>>
    {
        private HttpClient HttpClient { get; set; }

        public CreateTroopsTest(WebApplicationFactory<Startup> factory)
        {
            HttpClient = factory.CreateClient();
        }

        public string TestLoginReturnToken(string userName, string password)
        {
            var inputObj = JsonConvert.SerializeObject(new PlayerRequest() { Username = userName, Password = password });
            StringContent requestContent = new(inputObj, Encoding.UTF8, "application/json");
            var response = HttpClient.PostAsync("https://localhost:20625/login", requestContent).Result;
            string contentResponse = response.Content.ReadAsStringAsync().Result;
            LoginResponse token = JsonConvert.DeserializeObject<LoginResponse>(contentResponse);
            string tokenResult = token.Token;
            return tokenResult;
        }

        [Fact]
        public void HttpPostCreateTroops_ReturnsStatusOk()
        {
            //arrange
            var request = new HttpRequestMessage();
            var tokenResult = TestLoginReturnToken("Nya", "catcatcat");
            StatusResponse expectedStatusResult = new();
            expectedStatusResult.Status = "Ok";
            HttpStatusCode expectedStatusCode = HttpStatusCode.OK;

            TroopCreationRequest requestBody = new();
            requestBody.Type = "phalanx";
            requestBody.Quantity = 1;
            string requestBodyContent = JsonConvert.SerializeObject(requestBody);
            StringContent requestContent = new(requestBodyContent, Encoding.UTF8, "application/json");
            request.RequestUri = new Uri("http://localhost:20625/kingdoms/1/troops");
            request.Method = HttpMethod.Post;
            request.Content = requestContent;
            request.Headers.Add("authorization", $"bearer {tokenResult}");

            //act
            var response = HttpClient.SendAsync(request).Result;
            string responseBodyContent = response.Content.ReadAsStringAsync().Result;

            //assert
            Assert.Equal(expectedStatusCode, response.StatusCode);
        }

        [Fact]
        public void HttpPostCreateTroops_ReturnsUnauhtorizedAndError()
        {
            //arrange
            var request = new HttpRequestMessage();
            var tokenResult = TestLoginReturnToken("Nya", "catcatcat");
            ErrorResponse expectedErrorResult = new();
            expectedErrorResult.Error = "This kingdom does not belong to authenticated player";
            HttpStatusCode expectedStatusCode = HttpStatusCode.Unauthorized;

            TroopCreationRequest requestBody = new();
            requestBody.Type = "phalanx";
            requestBody.Quantity = 2;
            string requestBodyContent = JsonConvert.SerializeObject(requestBody);
            StringContent requestContent = new(requestBodyContent, Encoding.UTF8, "application/json");
            request.RequestUri = new Uri("http://localhost:20625/kingdoms/2/troops");
            request.Method = HttpMethod.Post;
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
        public void HttpPostCreateTroops_ReturnsBadRequestAndError()
        {
            //arrange
            var request = new HttpRequestMessage();
            var tokenResult = TestLoginReturnToken("Nya", "catcatcat");
            ErrorResponse expectedErrorResult = new();
            expectedErrorResult.Error = "You don't have enough gold to train all these units!";
            HttpStatusCode expectedStatusCode = HttpStatusCode.BadRequest;

            TroopCreationRequest requestBody = new();
            requestBody.Type = "senator";
            requestBody.Quantity = 20;
            string requestBodyContent = JsonConvert.SerializeObject(requestBody);
            StringContent requestContent = new(requestBodyContent, Encoding.UTF8, "application/json");
            request.RequestUri = new Uri("http://localhost:20625/kingdoms/1/troops");
            request.Method = HttpMethod.Post;
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

