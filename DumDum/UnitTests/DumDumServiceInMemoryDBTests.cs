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

namespace UnitTests
{
    public class DumDumServiceInMemoryDBTests : TestService
    {
        private IAuthenticateService IAuthenticateService { get; set; }
      
        [Fact]
        public void GetPlayerById_ReturnsPlayer_WhenPlayerExists()
        {
            //arrange
            var UnitOfWork = GetContextWithoutData();
            var DumDumService = new DumDumService(IAuthenticateService, UnitOfWork);
            var testPlayer = DumDumService.
                RegisterPlayerLogic(new PlayerRequest { KingdomName = "testk", Password = "123456789", Username = "user" }).Result;
            UnitOfWork.Kingdoms.Add(DumDumService.GetKingdomById(testPlayer.Item1.KingdomId).Result);
            UnitOfWork.Players.Add(DumDumService.GetPlayerByUsername(testPlayer.Item1.Username).Result);

            //act
            var actualPlayer = DumDumService.GetPlayerById(1);

            //assert
            Assert.Equal(testPlayer.Item1.Username, actualPlayer.Result.Username);
        }


        [Fact]
        public void RegisterKingdom_ReturnsStatusOkAndCorrectResponse_WhenCoordinatesAndKingdomIdProvided()
        {
            //arrange
            IOptions<AppSettings> AppSettings = Options.Create<AppSettings>(new AppSettings() { Key = "This is my sample key" });
            StatusResponse expectedStatusResult = new();
            expectedStatusResult.Status = "Ok";
            var tokenPlayer = TestLoginReturnTokenPlayerInMemoryDB();
            var AuthenticateService = new AuthenticateService(AppSettings, tokenPlayer.Result.Item2);
            var dumDumService = new DumDumService(AuthenticateService, tokenPlayer.Result.Item2);

            KingdomRegistrationRequest requestBody = new();
            requestBody.CoordinateY = 88;
            requestBody.CoordinateX = 88;
            requestBody.KingdomId = 1;

            //act
            var response = dumDumService.RegisterKingdom(tokenPlayer.Result.Item1, requestBody);

            //assert
            Assert.Equal(expectedStatusResult.Status, response.Result.Item1);
        }
    }
}
