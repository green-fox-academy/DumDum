using DumDum;
using DumDum.Models.JsonEntities;
using DumDum.Models.JsonEntities.Resources;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using Xunit;

namespace IntegrationTests
{
    public class ResourceControllerTests : TestService, IClassFixture<WebApplicationFactory<Startup>>
    {
        [Fact]
        public void Resources_ShouldReturnAuthenticatedKingdomResources_WhenRequestIsCorrect()
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
        public void Resources_ShouldReturnUnathorized_WhenCorrectPlayerNotLoggedIn()
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