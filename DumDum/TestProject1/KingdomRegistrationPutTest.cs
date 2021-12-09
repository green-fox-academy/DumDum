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
        public void HttpPutRegistration_ReturnsStatusOkAndCorrectJSON()
        {
            //arrange
            StatusResponse expectedStatusResult = new();
            expectedStatusResult.Status = "Ok";
            HttpStatusCode expectedStatusCode = HttpStatusCode.OK;

            KingdomJson requestBody = new();
            requestBody.CoordinateY = 50;
            requestBody.CoordinateX = 50;
            requestBody.KingdomId = 1;
            string requestBodyContent = JsonConvert.SerializeObject(requestBody);
            StringContent requestContent = new(requestBodyContent, Encoding.UTF8, "application/json");

            //act
            HttpResponseMessage response = HttpClient.PutAsync("registration", requestContent).Result;
            string responseBodyContent = response.Content.ReadAsStringAsync().Result;
            StatusResponse responseData = JsonConvert.DeserializeObject<StatusResponse>(responseBodyContent);

            //assert
            Assert.Equal(expectedStatusCode, response.StatusCode);
            Assert.Equal(expectedStatusResult.Status, responseData.Status);
        }

        [Fact]
        public void HttpPutRegistration_ReturnsBadRequestAndCorrectJson1()
        {
            //arrange
            ErrorResponse expectedError = new();
            expectedError.Error = "One or both coordinates are out of valid range(0 - 99).";
            HttpStatusCode expectedStatusCode = HttpStatusCode.BadRequest;

            KingdomJson requestBody = new();
            requestBody.CoordinateY = 104;
            requestBody.CoordinateX = 4;
            requestBody.KingdomId = 1;
            string requestBodyContent = JsonConvert.SerializeObject(requestBody);
            StringContent requestContent = new(requestBodyContent, Encoding.UTF8, "application/json");

            //act
            HttpResponseMessage response = HttpClient.PutAsync("registration", requestContent).Result;
            string responseBodyContent = response.Content.ReadAsStringAsync().Result;
            ErrorResponse responseData = JsonConvert.DeserializeObject<ErrorResponse>(responseBodyContent);

            //assert
            Assert.Equal(expectedStatusCode, response.StatusCode);
            Assert.Equal(expectedError.Error, responseData.Error);
        }

        [Fact]
        public void HttpPutRegistration_ReturnsBadRequestAndCorrectJson2()
        {
            //arrange
            ErrorResponse expectedResult = new();
            expectedResult.Error = "Given coordinates are already taken!";
            HttpStatusCode expectedStatusCode = HttpStatusCode.BadRequest;

            KingdomJson requestBody = new();
            requestBody.CoordinateY = 10;
            requestBody.CoordinateX = 10;
            requestBody.KingdomId = 1;
            string requestBodyContent = JsonConvert.SerializeObject(requestBody);
            StringContent requestContent = new(requestBodyContent, Encoding.UTF8, "application/json");

            //act
            HttpResponseMessage response = HttpClient.PutAsync("registration", requestContent).Result;
            string responseBodyContent = response.Content.ReadAsStringAsync().Result;
            ErrorResponse responseData = JsonConvert.DeserializeObject<ErrorResponse>(responseBodyContent);

            //assert
            Assert.Equal(expectedStatusCode, response.StatusCode);
            Assert.Equal(expectedResult.Error, responseData.Error);
        }
    }
}
