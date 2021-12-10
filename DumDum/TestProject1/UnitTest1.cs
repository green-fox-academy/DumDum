using System;
using System.Net;
using System.Net.Http;
using System.Text;
using DumDum;
using DumDum.Models.JsonEntities;
using DumDum.Services;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;

namespace TestProject1
{
    public class UnitTest1 :  IClassFixture<WebApplicationFactory<Startup>>
    {
        private HttpClient HttpClient { get; set; }

        public UnitTest1(WebApplicationFactory<Startup> factory)
        {
            HttpClient = factory.CreateClient();
        }
        [Fact]
        public void LoginTest_ReturnUser()
        {
            var request = new HttpRequestMessage();

            var inputObj = JsonConvert.SerializeObject(new PlayerRequest() {Username = "Nya", Password = "catcatcat"});
            StringContent requestContent = new(inputObj, Encoding.UTF8, "application/json");
            var response = HttpClient.PostAsync("https://localhost:5000/login", requestContent).Result;
            string contentResponse = response.Content.ReadAsStringAsync().Result;
            LoginResponse token = JsonConvert.DeserializeObject<LoginResponse>(contentResponse);
            string tokenResult = token.Token;

            var inputObj2 = JsonConvert.SerializeObject(new AuthRequest() {Token = tokenResult});
            StringContent requestContent2 = new(inputObj2, Encoding.UTF8, "application/json");
            request.RequestUri = new Uri("https://localhost:5000/auth");
            request.Method = HttpMethod.Post;
            request.Content = requestContent2;
            var response2 = HttpClient.SendAsync(request).Result;

            Assert.Equal(HttpStatusCode.OK, response2.StatusCode);
        }

        [Fact]
        public void AuthPostEndpoint_ShouldReturnInfoAboutPLayer()
        {
            string rulerExpected = "Nya";
            long kingdomIdExpected = 1;
            string kingdomNameExpected = "Nya Nya Land";
            var statusCodeExpected = HttpStatusCode.OK;
            
            var inputObj = JsonConvert.SerializeObject(new PlayerRequest() {Username = "Nya", Password = "catcatcat"});
            StringContent requestContent = new(inputObj, Encoding.UTF8, "application/json");
            var response = HttpClient.PostAsync("https://localhost:5000/login", requestContent).Result;
            string contentResponse = response.Content.ReadAsStringAsync().Result;
            LoginResponse token = JsonConvert.DeserializeObject<LoginResponse>(contentResponse);
            string tokenResult = token.Token;

            var inputObj2 = JsonConvert.SerializeObject(new AuthRequest() {Token = tokenResult});
            StringContent requestContent2 = new(inputObj2, Encoding.UTF8, "application/json");
            var response2 = HttpClient.PostAsync("https://localhost:5000/auth", requestContent2).Result;
            string contentResponse2 = response2.Content.ReadAsStringAsync().Result;
            AuthResponse player = JsonConvert.DeserializeObject<AuthResponse>(contentResponse2);

            Assert.Equal(rulerExpected, player.Ruler);
            Assert.Equal(kingdomIdExpected, player.KingdomId);
            Assert.Equal(kingdomNameExpected, player.KingdomName);
            Assert.Equal(statusCodeExpected, response2.StatusCode);
        }

        [Fact]
        public void RenameKingdom_ShouldReturnChangedName()
        {
            var request = new HttpRequestMessage();

            var inputObj = JsonConvert.SerializeObject(new PlayerJson() {Username = "Nya", Password = "catcatcat"});
            StringContent requestContent = new(inputObj, Encoding.UTF8, "application/json");
            var response = HttpClient.PostAsync("https://localhost:5000/login", requestContent).Result;
            string contentResponse = response.Content.ReadAsStringAsync().Result;
            LoginResponse token = JsonConvert.DeserializeObject<LoginResponse>(contentResponse);
            string tokenResult = token.Token;
            
            var inputObj2 = JsonConvert.SerializeObject(new KingdomRenameRequest() {KingdomName = "Hahalkovo"});
            StringContent requestContent2 = new(inputObj2, Encoding.UTF8, "application/json");
            request.RequestUri = new Uri("https://localhost:5000/kingdoms");
            request.Method = HttpMethod.Put;
            request.Content = requestContent2;
            request.Headers.Add("authorization", $"bearer {tokenResult}");
            var response2 = HttpClient.SendAsync(request).Result;
            
            Assert.Equal(HttpStatusCode.OK, response2.StatusCode);
        }
        [Fact]
        public void RenameKingdom_ShouldReturnUnauthorized()
        {
            var request = new HttpRequestMessage();
            
            var inputObj2 = JsonConvert.SerializeObject(new KingdomRenameRequest() {KingdomName = "Hahalkovo"});
            StringContent requestContent2 = new(inputObj2, Encoding.UTF8, "application/json");
            request.RequestUri = new Uri("https://localhost:5000/kingdoms");
            request.Method = HttpMethod.Put;
            request.Content = requestContent2;
            request.Headers.Add("authorization", "");
            var response2 = HttpClient.SendAsync(request).Result;
            
            Assert.Equal(HttpStatusCode.Unauthorized, response2.StatusCode);
        }
        [Fact]
        public void RenameKingdom_ShouldReturnBadRequest()
        {
            var request = new HttpRequestMessage();
            
            var inputObj = JsonConvert.SerializeObject(new PlayerJson() {Username = "Nya", Password = "catcatcat"});
            StringContent requestContent = new(inputObj, Encoding.UTF8, "application/json");
            var response = HttpClient.PostAsync("https://localhost:5000/login", requestContent).Result;
            string contentResponse = response.Content.ReadAsStringAsync().Result;
            LoginResponse token = JsonConvert.DeserializeObject<LoginResponse>(contentResponse);
            string tokenResult = token.Token;
            
            var inputObj2 = JsonConvert.SerializeObject(new KingdomRenameRequest() {KingdomName = ""});
            StringContent requestContent2 = new(inputObj2, Encoding.UTF8, "application/json");
            request.RequestUri = new Uri("https://localhost:5000/kingdoms");
            request.Method = HttpMethod.Put;
            request.Content = requestContent2;
            request.Headers.Add("authorization", $"bearer {tokenResult}");
            var response2 = HttpClient.SendAsync(request).Result;
            
            Assert.Equal(HttpStatusCode.BadRequest, response2.StatusCode);
        }
        
    }
}


