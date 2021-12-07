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
    public class KingdomRegistrationPutTest : IClassFixture<WebApplicationFactory<Startup>>
    {
        private HttpClient HttpClient { get; set; }

        public KingdomRegistrationPutTest(WebApplicationFactory<Startup> factory)
        {
            HttpClient = factory.CreateClient();
        }

        [Fact]
        public void HttpPut_Returns200AndResult()
        {
            //arrange
            KingdomJson expectedResult = new();
            expectedResult.Status = "Ok";

            KingdomJson requestBody = new();
            requestBody.CoordinateY = 4;
            requestBody.CoordinateX = 4;
            requestBody.KingdomId = 1;
            string requestBodyContent = JsonConvert.SerializeObject(requestBody);
            StringContent requestContent = new(requestBodyContent, Encoding.UTF8, "application/json");

            //act
            HttpResponseMessage response = HttpClient.PutAsync("registration", requestContent).Result;
            string responseBodyContent = response.Content.ReadAsStringAsync().Result;
            KingdomJson responseData = JsonConvert.DeserializeObject<KingdomJson>(responseBodyContent);

            //assert
            Assert.Equal(expectedResult.Status, responseData.Status);
        }


    }
}
