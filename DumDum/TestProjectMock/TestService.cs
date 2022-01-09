using DumDum;
using DumDum.Database;
using DumDum.Interfaces;
using DumDum.Models;
using DumDum.Models.JsonEntities;
using DumDum.Models.JsonEntities.Kingdom;
using DumDum.Models.JsonEntities.Login;
using DumDum.Models.JsonEntities.Player;
using DumDum.Repository;
using DumDum.Services;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Xunit;

namespace TestProjectMock
{
    public class TestService
    {
        private HttpClient HttpClient { get; set; }
        private IAuthenticateService IAuthenticateService { get; set; }
        private IOptions<AppSettings> AppSettings { get; set; }

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

        internal string TestLoginReturnTokenPlayerInMemoryDB(out IUnitOfWork unitOfWork)
        {
             unitOfWork = GetContextWithoutData();
            var dumDumService = new DumDumService(IAuthenticateService, unitOfWork);
            var testPlayerRequest = new PlayerRequest { KingdomName = "TestKingdom", Password = "TestPassword", Username = "TestUser" };
            dumDumService.RegisterPlayerLogic(testPlayerRequest, out _);
            IOptions<AppSettings> AppSettings = Options.Create<AppSettings>(new AppSettings() {Key= "This is my sample key" });
            var LoginService = new LoginService(AppSettings,  dumDumService, unitOfWork);
            var token = LoginService.
                Login(new LoginRequest { Username= testPlayerRequest.Username, Password = testPlayerRequest.Password }, out _);
            return token;
        }
    }
}


