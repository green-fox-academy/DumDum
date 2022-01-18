using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DumDum.Controllers;
using DumDum.Interfaces;
using DumDum.Interfaces.IRepositories;
using DumDum.Interfaces.IServices;
using DumDum.Models.Entities;
using DumDum.Models.JsonEntities;
using DumDum.Models.JsonEntities.Kingdom;
using DumDum.Models.JsonEntities.Player;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace UnitTests
{
    public class DumDumServiceMoqTests : TestService
    {
        private DumDumController dumDumController;
        private readonly Mock<IAuthenticateService> authenticateServiceMoq = new Mock<IAuthenticateService>();
        private Mock<IDumDumService> dumDumServiceMoq = new Mock<IDumDumService>();
        private readonly Mock<IDetailService> detailServiceMoq = new Mock<IDetailService>();
        private readonly Mock<ITimeService> timeServiceMoq = new Mock<ITimeService>();
        private readonly Mock<IUnitOfWork> unitOfWorkMoq = new Mock<IUnitOfWork>();

        [Fact]
        public void KingdomsList_ReturnsEmptyObject_WhenNoKingdomPresent()
        {
            // Arrange
            var kingdomsEmptyList = Task.FromResult(new KingdomsListResponse());
            dumDumServiceMoq.Setup(x => x.GetAllKingdoms()).
                Returns(kingdomsEmptyList);

            dumDumController = new DumDumController(dumDumServiceMoq.Object, authenticateServiceMoq.Object,
               detailServiceMoq.Object, unitOfWorkMoq.Object, timeServiceMoq.Object);
            // Act
            var actual = dumDumController.KingdomsList();

            // Assert
            Assert.IsType<OkObjectResult>(actual);
            Assert.Equal(StatusCodes.Status200OK, (actual as ObjectResult).StatusCode);
        }

        [Fact]
        public void RegisterKingdom_ReturnsOkStatus_WhenCoordinatesAndKingdomIdProvided()
        {
            //Arrange
            KingdomRegistrationRequest moqRequest = new();
            moqRequest.CoordinateY = 88;
            moqRequest.CoordinateX = 88;
            moqRequest.KingdomId = 1;

            (string, int) expectedStatusResponseTuple = new("Ok", 200);
            var expectedStatusResponseTask = Task.FromResult(expectedStatusResponseTuple);
            dumDumController = new DumDumController(dumDumServiceMoq.Object, authenticateServiceMoq.Object,
              detailServiceMoq.Object, unitOfWorkMoq.Object, timeServiceMoq.Object);
            dumDumServiceMoq.Setup(x => x.RegisterKingdom("moqToken", moqRequest))
                            .Returns(expectedStatusResponseTask);

            // Act
            var actualStatus = dumDumController.RegisterKingdom("moqToken", moqRequest);
            var actualStatusResponse = (actualStatus as ObjectResult).Value as StatusResponse;

            // Assert
            Assert.Equal(expectedStatusResponseTask.Result.Item1, actualStatusResponse.Status);
        }

        [Fact]
        public void ListingAllKingdoms_ReturnsCorrectDtoObject_WhenRequestIsCorrect()
        {
            var kingdoms = new KingdomsListResponse();

            kingdoms.Kingdoms = new List<KingdomResponse>()
            {
                new KingdomResponse()
                {
                KingdomId = 1,
                KingdomName = "Hahalkovo",
                Ruler = "Nya",
                Population = 0,
                Location = new Location()
                    {
                        CoordinateX = 10,
                        CoordinateY = 10,
                    }
                }
            };

            dumDumServiceMoq.Setup(x => x.GetAllKingdoms().Result).Returns(kingdoms);

            var actual = dumDumServiceMoq.Object.GetAllKingdoms().Result;

            Assert.IsType<KingdomsListResponse>(actual);
            Assert.Equal(kingdoms, actual);
        }

        [Fact]
        public void RegisterPlayerLogic_ReturnsCorrectDtoObject_WhenRequestIsCorrect()
        {
            (PlayerResponse, int) expectedResponse = new();
            expectedResponse.Item2 = 200;

            dumDumServiceMoq.Setup(x => x.RegisterPlayerLogic(new PlayerRequest()).Result).Returns(expectedResponse);

            var actual = dumDumServiceMoq.Object.RegisterPlayerLogic(new PlayerRequest());

            Assert.IsType<(PlayerResponse, int)>(actual.Result);
        }
    }
}
