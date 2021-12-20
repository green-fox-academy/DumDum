using System;
using System.Net;
using System.Net.Http;
using System.Text;
using DumDum;
using DumDum.Models.JsonEntities;
using DumDum.Models.JsonEntities.Buildings;
using DumDum.Models.JsonEntities.Login;
using DumDum.Models.JsonEntities.Player;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;

namespace TestProject1
{

    public class BuildingUpgradeTest : IClassFixture<WebApplicationFactory<Startup>>
    {
        readonly HttpClient HttpClient;
        
        public BuildingUpgradeTest(WebApplicationFactory<Startup> factory)
        {
            HttpClient = factory.CreateClient();
        }
        
        public string TestLoginReturnToken(string userName, string password)
        {
            var inputObj = JsonConvert.SerializeObject(new PlayerRequest() {Username = userName, Password = password});
            StringContent requestContent = new(inputObj, Encoding.UTF8, "application/json");
            var response = HttpClient.PostAsync("https://localhost:5000/login", requestContent).Result;
            string contentResponse = response.Content.ReadAsStringAsync().Result;
            LoginResponse token = JsonConvert.DeserializeObject<LoginResponse>(contentResponse);
            string tokenResult = token.Token;
            return tokenResult;
        }
        
        [Fact]
        public void UpgradeReturnsOk()
        {
            var request = new HttpRequestMessage();
            var tokenResult = TestLoginReturnToken("Nya", "catcatcat");

            var inputObj = JsonConvert.SerializeObject(new UpgradeBuildingRequest() { KingdomId = 1, BuildingId = 1}); 
            StringContent requestContent = new(inputObj, Encoding.UTF8, "application/json");
            request.RequestUri = new Uri("http://localhost:5000/kingdoms/1/buildings/1");
            request.Method = HttpMethod.Put;
            request.Content = requestContent;
            request.Headers.Add("authorization", $"bearer {tokenResult}");
            var response = HttpClient.SendAsync(request).Result;

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
        
        [Fact]
        public void UpgradeBuildingsReturns400Error()
        {
            var request = new HttpRequestMessage();
            var tokenResult = TestLoginReturnToken("Nya", "catcatcat");

            var inputObj = JsonConvert.SerializeObject(new UpgradeBuildingRequest() { KingdomId = 1, BuildingId = 1});
            StringContent requestContent = new(inputObj, Encoding.UTF8, "application/json");
            request.RequestUri = new Uri("http://localhost:5000/kingdoms/1/buildings/1");
            request.Method = HttpMethod.Put;
            request.Content = requestContent;
            request.Headers.Add("authorization", $"bearer {tokenResult}");
            var response = HttpClient.SendAsync(request).Result;
            string respond = response.Content.ReadAsStringAsync().Result;
            ErrorResponse error = JsonConvert.DeserializeObject<ErrorResponse>(respond);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Equal("Kingdom not found", error.Error);
            Assert.Equal("This kingdom does not belong to authenticated player!", error.Error);
            Assert.Equal("Your building is on maximal leve!.", error.Error);
            Assert.Equal("You don't have enough gold to upgrade that!", error.Error);
            Assert.Equal("Your building is not ready yet for update", error.Error);
            Assert.Equal("Your building can't have higher level than your townhall! Upgrade townhall first.", error.Error);
            
        }
        
        [Fact]
        public void UpgradeReturns401Error()
        {
            var request = new HttpRequestMessage();
            var tokenResult = TestLoginReturnToken("Nya", "cattcat");

            var inputObj = JsonConvert.SerializeObject(new UpgradeBuildingRequest() { KingdomId = 1, BuildingId = 1}); 
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