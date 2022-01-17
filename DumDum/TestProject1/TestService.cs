using DumDum;
using DumDum.Database;
using DumDum.Interfaces;
using DumDum.Repository;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;

namespace TestProject1
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

        internal UnitOfWork GetContextWithoutData()
        {
            var connectionStringBuilder = new SqliteConnectionStringBuilder { DataSource = ":memory:" };
            var connection = new SqliteConnection(connectionStringBuilder.ToString());
            var options = new DbContextOptionsBuilder<ApplicationDbContext>().UseSqlite(connection).Options;
            var context = new ApplicationDbContext(options);

            var unitOfWork = new UnitOfWork(context);
            context.Database.OpenConnection();
            context.Database.EnsureCreated();
            return unitOfWork;
        }

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


