using System;
using System.Net;
using System.Net.Http;
using System.Text;
using DumDum;
using DumDum.Models.JsonEntities;
using DumDum.Models.JsonEntities.Authorization;
using DumDum.Models.JsonEntities.Login;
using DumDum.Models.JsonEntities.Player;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;

namespace TestProject1
{
        public class AuthenticationTest : IClassFixture<WebApplicationFactory<Startup>>
    {
               private HttpClient HttpClient { get; set; }

        public AuthenticationTest(WebApplicationFactory<Startup> factory)
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
    }
}