using DumDum;
using DumDum.Models.JsonEntities;
using DumDum.Models.JsonEntities.Buildings;
using DumDum.Services;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace TestProject1
{
    public class BuildingTest : IClassFixture<WebApplicationFactory<Startup>>
    {
        readonly HttpClient HttpClient;
       

        public BuildingTest(WebApplicationFactory<Startup> fixture)
        {
            HttpClient = fixture.CreateClient();  //vytvori se klient
           
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
        public void BuildingReturnOk()
        {
            var request = new HttpRequestMessage();
            var tokenResult = TestLoginReturnToken("Nya", "catcatcat");

            var inputObj = JsonConvert.SerializeObject(new BuildingRequest() { Id = 1}); //vstupovat s parametry, jako controller
            StringContent requestContent = new(inputObj, Encoding.UTF8, "application/json");
            request.RequestUri = new Uri("http://localhost:20625/kingdoms/1/buildings");
            request.Method = HttpMethod.Get;
            request.Content = requestContent;
            request.Headers.Add("authorization", $"bearer {tokenResult}");
            var response = HttpClient.SendAsync(request).Result;

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public void BuildingReturnError()
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
        public void BuildingLeaderboardReturOKAndLeaderboardList()
        {

            //arrange

            HttpStatusCode expectedStatusCode = HttpStatusCode.OK;

            var request = new HttpRequestMessage();
            request.RequestUri = new Uri("https://localhost:20625/leaderboards/buildings");
            request.Method = HttpMethod.Get;

            BuildingsLeaderboardResponse requestBody = new BuildingsLeaderboardResponse();

            requestBody.Result = new List<BuildingPointsResponse>()
            {
                new BuildingPointsResponse()
                {
                    Ruler = "Nya",
                    Kingdom = "Nya Nya Land",
                    Buildings = 3,
                    Points = 4
                }
            };

            var requestBodyContent = JsonConvert.SerializeObject(requestBody);

            //act
            HttpResponseMessage response = HttpClient.SendAsync(request).Result;
            string responseData = response.Content.ReadAsStringAsync().Result;
            BuildingsLeaderboardResponse responseDataObj = JsonConvert.DeserializeObject<BuildingsLeaderboardResponse>(responseData);

            Assert.NotNull(responseDataObj);

            //assert
            Assert.Equal(expectedStatusCode, response.StatusCode);
            Assert.Equal(requestBody.Result[0].Ruler, responseDataObj.Result[0].Ruler);
            Assert.Equal(requestBody.Result[0].Kingdom, responseDataObj.Result[0].Kingdom);
            Assert.Equal(requestBody.Result[0].Buildings, responseDataObj.Result[0].Buildings);
            Assert.Equal(requestBody.Result[0].Points, responseDataObj.Result[0].Points);

        }
    }
}
