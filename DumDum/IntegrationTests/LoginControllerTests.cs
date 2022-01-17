using DumDum;
using DumDum.Models.JsonEntities;
using DumDum.Models.JsonEntities.Authorization;
using DumDum.Models.JsonEntities.Login;
using DumDum.Models.JsonEntities.Player;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using Xunit;

namespace IntegrationTests
{
    public class LoginControllerTests : TestService, IClassFixture<WebApplicationFactory<Startup>>
    {
        [Fact]
        public void Login_ShouldReturnPlayer_WhenPlayerExists()
        {
            var request = new HttpRequestMessage();
            var tokenResult = TestLoginReturnToken("Nya", "catcatcat");

            var inputObj = JsonConvert.SerializeObject(new AuthRequest() { Token = tokenResult });
            StringContent requestContent = new(inputObj, Encoding.UTF8, "application/json");
            request.RequestUri = new Uri("https://localhost:20625/auth");
            request.Method = HttpMethod.Post;
            request.Content = requestContent;
            var response = HttpClient.SendAsync(request).Result;

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public void Auth_ShouldReturnInfoAboutPLayer()
        {
            var tokenResult = TestLoginReturnToken("Nya", "catcatcat");

            var inputObj = JsonConvert.SerializeObject(new AuthRequest() { Token = tokenResult });
            StringContent requestContent = new(inputObj, Encoding.UTF8, "application/json");
            var response = HttpClient.PostAsync("https://localhost:20625/auth", requestContent).Result;
            string contentResponse = response.Content.ReadAsStringAsync().Result;
            AuthResponse player = JsonConvert.DeserializeObject<AuthResponse>(contentResponse);

            Assert.Equal("Nya", player.Ruler);
            Assert.Equal(1, player.KingdomId);
            Assert.Equal("Nya Nya Land", player.KingdomName);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public void Login_ShouldReturnStatusOkAndToken_WhenRequestIsCorrect()
        {
            var inputObj = JsonConvert.SerializeObject(new LoginRequest() { Username = "Nya", Password = "catcatcat" }); //vezmu json, ktery tam chci poslat akonvert. Serialize je z norm objektu do jsonu
            StringContent requestContent = new(inputObj, Encoding.UTF8, "application/json"); //udelam z toho spravnou klacuu jaky to ma enchoding; app/json je typ souboru

            var response = HttpClient.PostAsync("http://localhost:20625/login", requestContent).Result; //smazat. dělá to samé co ř.35
            string respond = response.Content.ReadAsStringAsync().Result;                              //ještě jednou na to čekám
            LoginResponse token = JsonConvert.DeserializeObject<LoginResponse>(respond);
            var resultFromToken = token.Token.Split('.');
            var result = resultFromToken[0];

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("Ok", token.Status);
            Assert.Equal("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9", result);
        }

        [Fact]
        public void Login_ShouldReturnBadRequest_WhenPasswordIsEmpty()
        {
            var inputObj = JsonConvert.SerializeObject(new LoginRequest() { Username = "Nya", Password = "" });
            StringContent requestContent = new(inputObj, Encoding.UTF8, "application/json");
            var response = HttpClient.PostAsync("http://localhost:20625/login", requestContent).Result;
            string respond = response.Content.ReadAsStringAsync().Result;
            ErrorResponse error = JsonConvert.DeserializeObject<ErrorResponse>(respond);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Equal("Field username and/or field password was empty!", error.Error);
        }

        [Fact]
        public void Login_ShouldReturnUnauthorized_WhenPasswordIsWrong()
        {
            var inputObj = JsonConvert.SerializeObject(new LoginRequest() { Username = "Nya", Password = "catcatcar" });
            StringContent requestContent = new(inputObj, Encoding.UTF8, "application/json");
            var response = HttpClient.PostAsync("http://localhost:20625/login", requestContent).Result;
            string respond = response.Content.ReadAsStringAsync().Result;
            ErrorResponse error = JsonConvert.DeserializeObject<ErrorResponse>(respond);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.Equal("Username and/or password was incorrect!", error.Error);
        }
    }
}