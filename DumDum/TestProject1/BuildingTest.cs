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

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}
