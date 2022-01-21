using DumDum;
using DumDum.Models.JsonEntities.Login;
using DumDum.Models.JsonEntities.Player;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;

namespace IntegrationTests
{
    public class TestService
    {
        internal HttpClient HttpClient { get; set; }

        public TestService()
        {
            var appFactory = new WebApplicationFactory<Startup>();
            HttpClient = appFactory.CreateClient();
        }

        public string TestLoginReturnToken(string userName, string password)
        {
            var inputObj = JsonConvert.SerializeObject(new PlayerRequest() { Username = userName, Password = password });
            StringContent requestContent = new(inputObj, Encoding.UTF8, "application/json");
            var response = HttpClient.PostAsync("https://localhost:20625/login", requestContent).Result;
            string contentResponse = response.Content.ReadAsStringAsync().Result;
            LoginResponse token = JsonConvert.DeserializeObject<LoginResponse>(contentResponse);
            string tokenResult = token.Token;
            return tokenResult;
        }
    }
}


