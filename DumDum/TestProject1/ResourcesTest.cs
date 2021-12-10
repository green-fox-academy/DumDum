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
    public class ResourcesTest : IClassFixture<WebApplicationFactory<Startup>>
    {
        private HttpClient HttpClient { get; set; }

        public ResourcesTest(WebApplicationFactory<Startup> factory)
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
        public void ResourcesReturnsAuthenticatedKingdom()
        {
            var request = new HttpRequestMessage();
            var tokenResult = TestLoginReturnToken("Nya", "catcatcat");

            var inputObj = JsonConvert.SerializeObject(new ResourceRequest() { Id = 1}); 
            StringContent requestContent = new(inputObj, Encoding.UTF8, "application/json");
            request.RequestUri = new Uri("http://localhost:20625/kingdoms/1/resources");
            request.Method = HttpMethod.Get;
            request.Content = requestContent;
            request.Headers.Add("authorization", $"bearer {tokenResult}");
            var response = HttpClient.SendAsync(request).Result;

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
        
        [Fact]
        public void ResourcesReturnsError()
        {
            var request = new HttpRequestMessage();
            var tokenResult = TestLoginReturnToken("Nya", "catcatcat");

            var inputObj = JsonConvert.SerializeObject(new ResourceRequest() { Id = 1 }); 
            StringContent requestContent = new(inputObj, Encoding.UTF8, "application/json");
            request.RequestUri = new Uri("http://localhost:20625/kingdoms/2/resources");
            request.Method = HttpMethod.Get;
            request.Content = requestContent;
            request.Headers.Add("authorization", $"bearer {tokenResult}");
            var response = HttpClient.SendAsync(request).Result;
            string respond = response.Content.ReadAsStringAsync().Result;
            ErrorResponse error = JsonConvert.DeserializeObject<ErrorResponse>(respond);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.Equal("This kingdom does not belong to authenticated player", error.Error);
        }
    }
}