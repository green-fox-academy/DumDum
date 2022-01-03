using System;
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
    public class KingdomRenameTest : IClassFixture<WebApplicationFactory<Startup>>
    {
               private HttpClient HttpClient { get; set; }

        public KingdomRenameTest(WebApplicationFactory<Startup> factory)
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
    }
}