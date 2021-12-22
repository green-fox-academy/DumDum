using System;
using System.Net;
using System.Net.Http;
using System.Text;
using DumDum;
using DumDum.Models.JsonEntities;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;

namespace TestProject1
{
    public class KingdomRegistrationTest : IClassFixture<WebApplicationFactory<Startup>>
    {
        private HttpClient HttpClient { get; set; }

        public KingdomRegistrationTest(WebApplicationFactory<Startup> factory)
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
        public void HttpPutRegistration_ReturnsStatusOkAndCorrectResponse()
        {
            //arrange
            var request = new HttpRequestMessage();
            var tokenResult = TestLoginReturnToken("Nya", "catcatcat");
            StatusResponse expectedStatusResult = new();
            expectedStatusResult.Status = "Ok";
            HttpStatusCode expectedStatusCode = HttpStatusCode.OK;

            KingdomRegistrationRequest requestBody = new();
            requestBody.CoordinateY = 88;
            requestBody.CoordinateX = 88;
            requestBody.KingdomId = 1;
            string requestBodyContent = JsonConvert.SerializeObject(requestBody);
            StringContent requestContent = new(requestBodyContent, Encoding.UTF8, "application/json");
            request.RequestUri = new Uri("http://localhost:20625/registration");
            request.Method = HttpMethod.Put;
            request.Content = requestContent;
            request.Headers.Add("authorization", $"bearer {tokenResult}");

            //act
            var response = HttpClient.SendAsync(request).Result;
            string responseBodyContent = response.Content.ReadAsStringAsync().Result;
            StatusResponse responseData = JsonConvert.DeserializeObject<StatusResponse>(responseBodyContent);

            //assert
            Assert.Equal(expectedStatusCode, response.StatusCode);
            Assert.Equal(expectedStatusResult.Status, responseData.Status);
        }

        [Fact]
        public void HttpPutRegistration_ReturnsBadRequestAndErrorResponse()
        {
            //arrange
            var request = new HttpRequestMessage();
            var tokenResult = TestLoginReturnToken("Nya", "catcatcat");
            ErrorResponse expectedError = new();
            expectedError.Error = "One or both coordinates are out of valid range(0 - 99).";
            HttpStatusCode expectedStatusCode = HttpStatusCode.BadRequest;

            KingdomRegistrationRequest requestBody = new();
            requestBody.CoordinateY = 100;
            requestBody.CoordinateX = 88;
            requestBody.KingdomId = 1;
            string requestBodyContent = JsonConvert.SerializeObject(requestBody);
            StringContent requestContent = new(requestBodyContent, Encoding.UTF8, "application/json");
            request.RequestUri = new Uri("http://localhost:20625/registration");
            request.Method = HttpMethod.Put;
            request.Content = requestContent;
            request.Headers.Add("authorization", $"bearer {tokenResult}");

            //act
            var response = HttpClient.SendAsync(request).Result;
            string responseBodyContent = response.Content.ReadAsStringAsync().Result;
            ErrorResponse responseData = JsonConvert.DeserializeObject<ErrorResponse>(responseBodyContent);

            //assert
            Assert.Equal(expectedStatusCode, response.StatusCode);
            Assert.Equal(expectedError.Error, responseData.Error);
        }

        [Fact]
        public void HttpPutRegistration_ReturnsUnauthorizedtAndErrorResponse()
        {
            //arrange
            var request = new HttpRequestMessage();
            var tokenResult = TestLoginReturnToken("Nya", "catcatcat");
            ErrorResponse expectedError = new();
            expectedError.Error = "This kingdom does not belong to authenticated player";
            HttpStatusCode expectedStatusCode = HttpStatusCode.Unauthorized;

            KingdomRegistrationRequest requestBody = new();
            requestBody.CoordinateY = 88;
            requestBody.CoordinateX = 88;
            requestBody.KingdomId = 3;
            string requestBodyContent = JsonConvert.SerializeObject(requestBody);
            StringContent requestContent = new(requestBodyContent, Encoding.UTF8, "application/json");
            request.RequestUri = new Uri("http://localhost:20625/registration");
            request.Method = HttpMethod.Put;
            request.Content = requestContent;
            request.Headers.Add("authorization", $"bearer {tokenResult}");

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
