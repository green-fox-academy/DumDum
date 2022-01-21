using DumDum.Controllers;
using DumDum.Interfaces;
using DumDum.Interfaces.IServices;
using DumDum.Models;
using DumDum.Models.JsonEntities;
using DumDum.Models.JsonEntities.Authorization;
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
        private IAuthenticateService AuthenticateService { get; set; }
      

        [Fact]
        public void RegisterKingdom_ReturnsStatusOk_WhenCoordinatesAndKingdomIdProvided()
        {
            //arrange
            IOptions<AppSettings> AppSettings = Options.Create<AppSettings>(new AppSettings() { Key = "This is my sample key" });
            var tokenPlayer = TestLoginReturnTokenPlayerInMemoryDB();
            var AuthenticateService = new AuthenticateService(AppSettings, tokenPlayer.Result.Item2);
            var dumDumService = new DumDumService(AuthenticateService, tokenPlayer.Result.Item2);
            StatusResponse expectedStatusResult = new();
            expectedStatusResult.Status = "Ok";

            KingdomRegistrationRequest requestBody = new();
            requestBody.CoordinateY = 88;
            requestBody.CoordinateX = 88;
            requestBody.KingdomId = 1;

            //act
            var response = dumDumService.RegisterKingdom(tokenPlayer.Result.Item1, requestBody);

            //assert
            Assert.Equal(expectedStatusResult.Status, response.Result.Item1);
        }

        [Fact]
        public void GetPlayerById_ReturnsPlayer_WhenPlayerExists()
        {
            //arrange
            var UnitOfWork = AddPlayerToMemoryDB_ReturnContext();
            var DumDumService = new DumDumService(AuthenticateService, UnitOfWork);
            var expectedPlayer = UnitOfWork.Players.GetPlayerById(1);
       
            //act
            var actualPlayer = DumDumService.GetPlayerById(1);

            //assert
            Assert.Equal(expectedPlayer.Result.Username, actualPlayer.Result.Username);
        }

    }
}
