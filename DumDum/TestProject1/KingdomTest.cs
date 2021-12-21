using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using DumDum;
using DumDum.Models.JsonEntities;
using DumDum.Models.JsonEntities.Kingdom;
using DumDum.Models.JsonEntities.Login;
using DumDum.Models.JsonEntities.Player;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;

namespace TestProject1
{
    public class KingdomTest : IClassFixture<WebApplicationFactory<Startup>>
    {
        private HttpClient HttpClient { get; set; }

        public KingdomTest(WebApplicationFactory<Startup> factory)
        {
            HttpClient = factory.CreateClient();
        }

        public string TestLoginReturnToken(string userName, string password)
        {
            var inputObj = JsonConvert.SerializeObject(new PlayerRequest() { Username = userName, Password = password });
            StringContent requestContent = new(inputObj, Encoding.UTF8, "application/json");
            var response = HttpClient.PostAsync("https://localhost:5000/login", requestContent).Result;
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
        public void HttpPutRegistration_ReturnsBadRequestAndCorrectResponse1()
        {
            //arrange
            var request = new HttpRequestMessage();
            var tokenResult = TestLoginReturnToken("Nya", "catcatcat");
            ErrorResponse expectedError = new();
            expectedError.Error = "One or both coordinates are out of valid range(0 - 99).";
            HttpStatusCode expectedStatusCode = HttpStatusCode.BadRequest;

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
            ErrorResponse responseData = JsonConvert.DeserializeObject<ErrorResponse>(responseBodyContent);

            //assert
            Assert.Equal(expectedStatusCode, response.StatusCode);
            Assert.Equal(expectedError.Error, responseData.Error);
        }

        [Fact]
        public void HttpPutRegistration_ReturnsBadRequestAndCorrectResponse2()
        {
            //arrange
            var request = new HttpRequestMessage();
            var tokenResult = TestLoginReturnToken("Nya", "catcatcat");
            ErrorResponse expectedError = new();
            expectedError.Error = "Given coordinates are already taken!";
            HttpStatusCode expectedStatusCode = HttpStatusCode.BadRequest;

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
            ErrorResponse responseData = JsonConvert.DeserializeObject<ErrorResponse>(responseBodyContent);

            //assert
            Assert.Equal(expectedStatusCode, response.StatusCode);
            Assert.Equal(expectedError.Error, responseData.Error);
        }
        [Fact]
        public void ListingAllKingdoms_ReturnsOKAndCorrectJson()
        {
            //arrange

            HttpStatusCode expectedStatusCode = HttpStatusCode.OK;

            var request = new HttpRequestMessage();
            request.RequestUri = new Uri("https://localhost:20625/kingdoms");
            request.Method = HttpMethod.Get;

            KingdomsListResponse requestBody = new KingdomsListResponse();

            requestBody.Kingdoms = new List<KingdomResponse>()
            {
                new KingdomResponse()
                {
                KingdomId = 1,
                KingdomName = "Nya Nya Land",
                Ruler = "Nya",
                Population = 0,
                Location = new DumDum.Models.Entities.Location()
                    {
                        CoordinateX = 10,
                        CoordinateY = 10,
                    }
                }
            };

            var requestBodyContent = JsonConvert.SerializeObject(requestBody);

            //act
            HttpResponseMessage response = HttpClient.SendAsync(request).Result;
            string responseData = response.Content.ReadAsStringAsync().Result;
            KingdomsListResponse responseDataObj = JsonConvert.DeserializeObject<KingdomsListResponse>(responseData);

            Assert.NotNull(responseDataObj);

            //assert
            Assert.Equal(expectedStatusCode, response.StatusCode);
            Assert.Equal(requestBody.Kingdoms[0].KingdomName,responseDataObj.Kingdoms[0].KingdomName);

        }
    }
}
