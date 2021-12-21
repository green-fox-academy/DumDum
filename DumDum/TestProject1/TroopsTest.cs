using DumDum;
using DumDum.Models.JsonEntities;
using DumDum.Models.JsonEntities.Buildings;
using DumDum.Models.JsonEntities.Troops;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using Xunit;

namespace TestProject1
{
    public class TroopsTest : IClassFixture<WebApplicationFactory<Startup>>
    {
        readonly HttpClient HttpClient;


        public TroopsTest(WebApplicationFactory<Startup> fixture)
        {
            HttpClient = fixture.CreateClient(); 
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
        public void TroopsReturnOk()
        {
            var request = new HttpRequestMessage();
            var tokenResult = TestLoginReturnToken("Nya", "catcatcat");

            var inputObj = JsonConvert.SerializeObject(new  { kingdomId = 1 });
            StringContent requestContent = new(inputObj, Encoding.UTF8, "application/json");
            request.RequestUri = new Uri("http://localhost:20625/kingdoms/1/troops");
            request.Method = HttpMethod.Get;
            request.Content = requestContent;
            request.Headers.Add("authorization", $"bearer {tokenResult}");
            var response = HttpClient.SendAsync(request).Result;

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public void TroopsReturnError()
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
        public void TroopsLeaderboardReturOKAndLeaderboardList()
        {

            //arrange

            HttpStatusCode expectedStatusCode = HttpStatusCode.OK;

            var request = new HttpRequestMessage();
            request.RequestUri = new Uri("https://localhost:20625/leaderboards/troops");
            request.Method = HttpMethod.Get;

            TroopsLeaderboardResponse requestBody = new TroopsLeaderboardResponse();

            requestBody.Result = new List<TroopsPointResponse>()
            {
                new TroopsPointResponse()
                {
                    Ruler = "Nya",
                    Kingdom = "Nya Nya Land",
                    Troops = 5,
                    Points = 11.8
                }
            };

            //act
            HttpResponseMessage response = HttpClient.SendAsync(request).Result;
            string responseData = response.Content.ReadAsStringAsync().Result;
            TroopsLeaderboardResponse responseDataObj = JsonConvert.DeserializeObject<TroopsLeaderboardResponse>(responseData);

            Assert.NotNull(responseDataObj);

            //assert
            Assert.Equal(expectedStatusCode, response.StatusCode);
            Assert.Equal(requestBody.Result[0].Ruler, responseDataObj.Result[0].Ruler);
            Assert.Equal(requestBody.Result[0].Kingdom, responseDataObj.Result[0].Kingdom);
            Assert.Equal(requestBody.Result[0].Troops, responseDataObj.Result[0].Troops);
            Assert.Equal(requestBody.Result[0].Points, responseDataObj.Result[0].Points);

        }
    }
}
