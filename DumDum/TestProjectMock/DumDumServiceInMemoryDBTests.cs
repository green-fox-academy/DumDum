using DumDum.Controllers;
using DumDum.Interfaces;
using DumDum.Models;
using DumDum.Models.JsonEntities;
using DumDum.Models.JsonEntities.Kingdom;
using DumDum.Models.JsonEntities.Player;
using DumDum.Services;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace TestProjectMock
{
    public class DumDumServiceInMemoryDBTests : TestService
    {
        private IAuthenticateService IAuthenticateService { get; set; }
        private DumDumController dumDumController;
        private readonly Mock<IAuthenticateService> authenticateServiceMoq = new Mock<IAuthenticateService>();
        private Mock<IDumDumService> dumDumServiceMoq = new Mock<IDumDumService>();
        private readonly Mock<IDetailService> detailServiceMoq = new Mock<IDetailService>();
        private readonly Mock<ITimeService> timeServiceMoq = new Mock<ITimeService>();
        private readonly Mock<IUnitOfWork> unitOfWorkMoq = new Mock<IUnitOfWork>();

        public DumDumServiceInMemoryDBTests()
        {
        
        }

        [Fact]
        public void GetPlayerById_ReturnsPlayer()
        {
            //arrange
            var UnitOfWork = GetContextWithoutData();
            var DumDumService = new DumDumService(IAuthenticateService, UnitOfWork);
            var testPlayer = DumDumService.
                RegisterPlayerLogic(new PlayerRequest { KingdomName = "testk", Password = "123456789", Username = "user" }, out int StatusCode);
            UnitOfWork.Kingdoms.Add(DumDumService.GetKingdomById(testPlayer.KingdomId));
            UnitOfWork.Players.Add(DumDumService.GetPlayerByUsername(testPlayer.Username));

            //act
            var actualPlayer = DumDumService.GetPlayerById(1);

            //assert
            Assert.Equal(testPlayer.Username, actualPlayer.Username);
        }

        [Fact]
        public void RegisterKingdom_ReturnsStatusOkAndCorrectResponse()
        {
            //arrange
            IOptions<AppSettings> AppSettings = Options.Create<AppSettings>(new AppSettings() { Key = "This is my sample key" });
            StatusResponse expectedStatusResult = new();
            expectedStatusResult.Status = "Ok";
            string token = TestLoginReturnTokenPlayerInMemoryDB(out IUnitOfWork unitOfWork);
             var AuthenticateService = new AuthenticateService(AppSettings, unitOfWork);
            var dumDumService = new DumDumService(AuthenticateService, unitOfWork);

            KingdomRegistrationRequest requestBody = new();
            requestBody.CoordinateY = 88;
            requestBody.CoordinateX = 88;
            requestBody.KingdomId = 1;

            //act
            var response = dumDumService.RegisterKingdom(token, requestBody, out int StatusCode);
            
            //assert
            Assert.Equal(expectedStatusResult.Status, response);
        }
    }
}
