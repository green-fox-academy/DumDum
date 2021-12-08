using System;
using System.Net;
using System.Net.Http;
using System.Text;
using DumDum;
using DumDum.Controllers;
using DumDum.Models.JsonEntities;
using DumDum.Services;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;

namespace TestProject1
{
    public class LoginTest : IClassFixture<WebApplicationFactory<Startup>>
    {
        readonly HttpClient client;

        public LoginTest(WebApplicationFactory<Startup> fixture)
        {
            client = fixture.CreateClient();
        }

        [Fact]
        public void LoginTestReturnOkAndToken()
        {
            var request = new HttpRequestMessage();
            var inputObj = JsonConvert.SerializeObject(new LoginRequest() { Username = "Beef69", Password = "chicken" });
            
            StringContent requestContent = new(inputObj, Encoding.UTF8, "application/json");
            
            request.RequestUri = new Uri("http://localhost:20625/login");
            request.Method = HttpMethod.Post;
            request.Content = requestContent;
            var response = client.SendAsync(request).Result;

            var response2 = client.PostAsync("http://localhost:20625/login", requestContent).Result;
            string respond = response2.Content.ReadAsStringAsync().Result;
            LoginResponse token = JsonConvert.DeserializeObject<LoginResponse>(respond);
            var resultFromToken = token.Token.Split('.');
            var result = resultFromToken[0];

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("Ok", token.Status);
            Assert.Equal("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9", result);
        }

        [Fact]
        public void LoginTestWithEmptyUserNameOrPassword()
        {
            
            var request = new HttpRequestMessage();
            var inputObj = JsonConvert.SerializeObject(new LoginRequest() { Username = "", Password = "chicken"});
            StringContent requestContent = new(inputObj, Encoding.UTF8, "application/json");
            request.RequestUri = new Uri("http://localhost:20625/login");
            request.Method = HttpMethod.Post;
            request.Content = requestContent;
            var response = client.SendAsync(request).Result;
            
            var response2 = client.PostAsync("http://localhost:20625/login", requestContent).Result;
            string respond = response2.Content.ReadAsStringAsync().Result;
            LoginResponse token = JsonConvert.DeserializeObject<LoginResponse>(respond);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Equal("Field username and/or field password was empty!", token.Error);
            

        }

        [Fact]
        public void LoginTestWithWrongUserOrPassword()
        {
            var request = new HttpRequestMessage();
            var inputObj = JsonConvert.SerializeObject(new LoginRequest() { Username = "Beef", Password = "chicken" });
            StringContent requestContent = new(inputObj, Encoding.UTF8, "application/json");
            request.RequestUri = new Uri("http://localhost:20625/login");
            request.Method = HttpMethod.Post;
            request.Content = requestContent;
            var response = client.SendAsync(request).Result;
            var response2 = client.PostAsync("http://localhost:20625/login", requestContent).Result;
            string respond = response2.Content.ReadAsStringAsync().Result;
            LoginResponse token = JsonConvert.DeserializeObject<LoginResponse>(respond);


            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.Equal("Username and/or password was incorrect!", token.Error);
        }


    }
}
