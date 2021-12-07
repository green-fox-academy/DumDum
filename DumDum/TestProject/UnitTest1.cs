using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using DumDum;
using DumDum.Services;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace TestProject
{
    public class UnitTest1 : IClassFixture<WebApplicationFactory<Startup>>
    {
        private HttpClient HttpClient { get; set; }

        public UnitTest1(WebApplicationFactory<Startup> factory)
        {
            HttpClient = factory.CreateClient();
        }
    
        [Fact]
        public void LoginTest_ReturnUser()
        {
            //Arrange
            HttpStatusCode expectedStatusCode = HttpStatusCode.OK;
            //HttpStatusCode expectedStatusCode2 = HttpStatusCode.NotFound;
        
            //string expectedMessage1 = "Field username and/or field password was empty!";
            //string expectedMessage2 = "Username and / or password was incorrect!";
            StringContent requestContext = new StringContent("nick", Encoding.UTF8, "application/json");
        
            //Act 
            var response = HttpClient.PostAsync("login", requestContext ).Result;
        
            //Assert
            Assert.Equal(expectedStatusCode, response.StatusCode);
        }
    }
}

