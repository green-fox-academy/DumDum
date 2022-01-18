using DumDum;
using DumDum.Database;
using DumDum.Interfaces;
using DumDum.Models;
using DumDum.Models.JsonEntities.Login;
using DumDum.Models.JsonEntities.Player;
using DumDum.Repository;
using DumDum.Services;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Helpers;

namespace UnitTests
{
    public class TestService
    {
        internal HttpClient HttpClient { get; set; }
        private IAuthenticateService AuthenticateService { get; set; }
        private IOptions<AppSettings> AppSettings { get; set; }

        public TestService()
        {
            var appFactory = new WebApplicationFactory<Startup>();
            HttpClient = appFactory.CreateClient();
            AppSettings = Options.Create<AppSettings>(new AppSettings() { Key = "This is my sample key" });
        }

        internal UnitOfWork AddPlayerToMemoryDB_ReturnContext()
        {
            var connectionStringBuilder = new SqliteConnectionStringBuilder { DataSource = ":memory:" };
            var connection = new SqliteConnection(connectionStringBuilder.ToString());
            var options = new DbContextOptionsBuilder<ApplicationDbContext>().UseSqlite(connection).Options;
            var context = new ApplicationDbContext(options);

            var unitOfWork = new UnitOfWork(context);
            context.Database.OpenConnection();
            context.Database.EnsureCreated();
            unitOfWork.Players.Add(new DumDum.Models.Entities.Player()
            {
                Username = "TestUser",
                Password = Crypto.HashPassword("Password1"),
                Email = "Test@email.com",
                IsVerified = true,
                KingdomId = 1,
                PlayerId = 1,
                Kingdom = new DumDum.Models.Entities.Kingdom() { KingdomId = 1, PlayerId = 1, KingdomName = "TestKingdom" }
            });
            unitOfWork.Complete();
            return unitOfWork;
        }

        internal async Task<(string, IUnitOfWork)> TestLoginReturnTokenPlayerInMemoryDB()
        {
            var unitOfWork = AddPlayerToMemoryDB_ReturnContext();
            var dumDumService = new DumDumService(AuthenticateService, unitOfWork);
            var LoginService = new LoginService(AppSettings, dumDumService, unitOfWork);

            var playerFromDb = unitOfWork.Players.GetPlayerById(1).Result;
            var token = LoginService.
                Login(new LoginRequest { Username = playerFromDb.Username, Password = "Password1" }
                ).Result.Item1;
            return (token, unitOfWork);
        }
    }
}


