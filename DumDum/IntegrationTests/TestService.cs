using DumDum;
using DumDum.Database;
using DumDum.Interfaces;
using DumDum.Models.JsonEntities.Login;
using DumDum.Models.JsonEntities.Player;
using DumDum.Repository;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;

namespace IntegrationTests
{
    public class TestService
    {
        internal HttpClient HttpClient { get; set; }
        private IAuthenticateService IAuthenticateService { get; set; }

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

        //internal UnitOfWork GetContextWithoutData()
        //{
        //    var connectionStringBuilder = new SqliteConnectionStringBuilder { DataSource = ":memory:" };
        //    var connection = new SqliteConnection(connectionStringBuilder.ToString());
        //    var options = new DbContextOptionsBuilder<ApplicationDbContext>().UseSqlite(connection).Options;
        //    var context = new ApplicationDbContext(options);

        //    var unitOfWork = new UnitOfWork(context);
        //    context.Database.OpenConnection();
        //    context.Database.EnsureCreated();
        //    return unitOfWork;
        //}

        /*
        internal string TestLoginReturnTokenPlayerInMemoryDB(out IUnitOfWork unitOfWork)
        {
             unitOfWork = GetContextWithoutData();
            var dumDumService = new DumDumService(IAuthenticateService, unitOfWork);
            var testPlayerRequest = new PlayerRequest { KingdomName = "TestKingdom", Password = "TestPassword", Username = "TestUser" };
            dumDumService.RegisterPlayerLogic(testPlayerRequest);
            unitOfWork.Complete();
            IOptions<AppSettings> AppSettings = Options.Create<AppSettings>(new AppSettings() {Key= "This is my sample key" });
            var LoginService = new LoginService(AppSettings,  dumDumService, unitOfWork);
            var token = LoginService.
                Login(new LoginRequest { Username= testPlayerRequest.Username, Password = testPlayerRequest.Password });
            return token;
        }*/
    }
}


