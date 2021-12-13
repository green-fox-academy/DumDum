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
        public void LoginTest_ReturnUser()
        {
            var request = new HttpRequestMessage();
            var tokenResult = TestLoginReturnToken("Nya", "catcatcat");
            
            var inputObj = JsonConvert.SerializeObject(new AuthRequest() {Token = tokenResult});
            StringContent requestContent = new(inputObj, Encoding.UTF8, "application/json");
            request.RequestUri = new Uri("https://localhost:5000/auth");
            request.Method = HttpMethod.Post;
            request.Content = requestContent;
            var response = HttpClient.SendAsync(request).Result;

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public void AuthPostEndpoint_ShouldReturnInfoAboutPLayer()
        {
            var statusCodeExpected = HttpStatusCode.OK;
            var tokenResult = TestLoginReturnToken("Nya", "catcatcat");
            
            var inputObj = JsonConvert.SerializeObject(new AuthRequest() {Token = tokenResult});
            StringContent requestContent = new(inputObj, Encoding.UTF8, "application/json");
            var response = HttpClient.PostAsync("https://localhost:5000/auth", requestContent).Result;
            string contentResponse = response.Content.ReadAsStringAsync().Result;
            AuthResponse player = JsonConvert.DeserializeObject<AuthResponse>(contentResponse);

            Assert.Equal("Nya", player.Ruler);
            Assert.Equal(1, player.KingdomId);
            Assert.Equal("Nya Nya Land", player.KingdomName);
            Assert.Equal(statusCodeExpected, response.StatusCode);
        }

        [Fact]
        public void RenameKingdom_ShouldReturnChangedName()
        {
            var request = new HttpRequestMessage();
            var tokenResult = TestLoginReturnToken("Nya", "catcatcat");
            
            var inputObj = JsonConvert.SerializeObject(new KingdomRenameRequest() {KingdomName = "Hahalkovo"});
            StringContent requestContent = new(inputObj, Encoding.UTF8, "application/json");
            request.RequestUri = new Uri("https://localhost:5000/kingdoms");
            request.Method = HttpMethod.Put;
            request.Content = requestContent;
            request.Headers.Add("authorization", $"bearer {tokenResult}");
            var response = HttpClient.SendAsync(request).Result;
            
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
        [Fact]
        public void RenameKingdom_ShouldReturnUnauthorized()
        {
            var request = new HttpRequestMessage();
            
            var inputObj = JsonConvert.SerializeObject(new KingdomRenameRequest() {KingdomName = "Hahalkovo"});
            StringContent requestContent = new(inputObj, Encoding.UTF8, "application/json");
            request.RequestUri = new Uri("https://localhost:5000/kingdoms");
            request.Method = HttpMethod.Put;
            request.Content = requestContent;
            var response = HttpClient.SendAsync(request).Result;
            
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
        [Fact]
        public void RenameKingdom_ShouldReturnBadRequest()
        {
            var request = new HttpRequestMessage();
            var tokenResult = TestLoginReturnToken("Nya", "catcatcat");
            
            var inputObj = JsonConvert.SerializeObject(new KingdomRenameRequest() {KingdomName = ""});
            StringContent requestContent = new(inputObj, Encoding.UTF8, "application/json");
            request.RequestUri = new Uri("https://localhost:5000/kingdoms");
            request.Method = HttpMethod.Put;
            request.Content = requestContent;
            request.Headers.Add("authorization", $"bearer {tokenResult}");
            var response = HttpClient.SendAsync(request).Result;
            
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public void KingdomDetails_ShouldReturnOk()
        {
            var request = new HttpRequestMessage();
            var tokenResult = TestLoginReturnToken("Nya", "catcatcat");
            
            var inputObj = JsonConvert.SerializeObject(new KingdomDetailsRequest(){Id = 1});
            StringContent requestContent = new(inputObj, Encoding.UTF8, "application/json");
            request.RequestUri = new Uri("https://localhost:5000/kingdoms/1/");
            request.Method = HttpMethod.Get;
            request.Content = requestContent;
            request.Headers.Add("authorization", $"bearer {tokenResult}");
            var response = HttpClient.SendAsync(request).Result;
            
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
        [Fact]
        public void KingdomDetails_ShouldReturnUnauthorized()
        {
            var request = new HttpRequestMessage();

            var inputObj = JsonConvert.SerializeObject(new KingdomDetailsRequest(){Id = 1});
            StringContent requestContent = new(inputObj, Encoding.UTF8, "application/json");
            request.RequestUri = new Uri("https://localhost:5000/kingdoms/1/");
            request.Method = HttpMethod.Get;
            request.Content = requestContent;
            var response = HttpClient.SendAsync(request).Result;
            
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
        
    }
}


