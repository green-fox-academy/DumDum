using System;
using System.Net;
using System.Net.Http;
using System.Text;
using DumDum;
using DumDum.Models.JsonEntities.Player;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;

namespace TestProject1
{
    public class RegisterTest :  IClassFixture<WebApplicationFactory<Startup>>
    {
        private HttpClient HttpClient { get; set; }

        public RegisterTest(WebApplicationFactory<Startup> factory)
        {
            HttpClient = factory.CreateClient();
        }

        [Fact]
        public void RegistrationReturnsOk()
        {
            var request = new HttpRequestMessage();

            var inputObj = JsonConvert.SerializeObject(new PlayerRequest() { Username = "Nyan", Password = "nanynnayn" });
            StringContent requestContent = new(inputObj, Encoding.UTF8, "application/json");
            request.RequestUri = new Uri("http://localhost:5467/registration");
            request.Method = HttpMethod.Post;
            request.Content = requestContent;
            var response = HttpClient.SendAsync(request).Result;

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
        
        [Fact]
        public void RegistrationReturnsBadRequest()
        {
            var request = new HttpRequestMessage();

            var inputObj = JsonConvert.SerializeObject(new PlayerRequest() { Username = "", Password = "ayn"});
            StringContent requestContent = new(inputObj, Encoding.UTF8, "application/json");
            request.RequestUri = new Uri("http://localhost:5467/registration");
            request.Method = HttpMethod.Post;
            request.Content = requestContent;
            var response = HttpClient.SendAsync(request).Result;
            
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}