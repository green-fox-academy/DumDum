using System.Collections.Generic;
using System.Threading.Tasks;
using DumDum.Controllers;
using DumDum.Interfaces;
using DumDum.Models.JsonEntities;
using DumDum.Models.JsonEntities.Kingdom;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace TestProject1
{
    public class DumDumServiceMoqTests : TestService
    {
        private IAuthenticateService IAuthenticateService { get; set; }
        private DumDumController dumDumController;
        private readonly Mock<IAuthenticateService> authenticateServiceMoq = new Mock<IAuthenticateService>();
        private Mock<IDumDumService> dumDumServiceMoq = new Mock<IDumDumService>();
        private Mock<ITroopService> troopServiceMoq = new Mock<ITroopService>();
        private readonly Mock<IDetailService> detailServiceMoq = new Mock<IDetailService>();
        private readonly Mock<ITimeService> timeServiceMoq = new Mock<ITimeService>();
        private readonly Mock<IUnitOfWork> unitOfWorkMoq = new Mock<IUnitOfWork>();

        //[Fact]
        //public void KingdomsList_ReturnsEmptyObject_WhenNoKingdomPresent()
        //{
        //    // Arrange
        //    KingdomsListResponse kingdomsEmptyList = new();
        //    dumDumServiceMoq.Setup(x => x.GetAllKingdoms()).Returns(new Task<KingdomsListResponse>() { Kingdoms = kingdomsEmptyList });
        //    dumDumController = new DumDumController(dumDumServiceMoq.Object, authenticateServiceMoq.Object,
        //       detailServiceMoq.Object, unitOfWorkMoq.Object, timeServiceMoq.Object);
        //    // Act
        //    var actual = dumDumController.KingdomsList();

        //    // Assert
        //    Assert.IsType<OkObjectResult>(actual);
        //    Assert.Equal(StatusCodes.Status200OK, (actual as ObjectResult).StatusCode);
        //}

        /*[Fact]
        public void RegisterKingdom_ReturnsOkStatus_WhenCoordinatesAndKingdomIdProvided()
        {
            // Arrange
            int statusCode = 200;
            KingdomRegistrationRequest moqRequest = new();
            moqRequest.CoordinateY = 88;
            moqRequest.CoordinateX = 88;
            moqRequest.KingdomId = 1;
            var expectedStatusResponse = new StatusResponse { Status = "Ok" };
            dumDumController = new DumDumController(dumDumServiceMoq.Object, authenticateServiceMoq.Object,
              detailServiceMoq.Object, unitOfWorkMoq.Object, timeServiceMoq.Object);

            dumDumServiceMoq.Setup(x => x.RegisterKingdom("moqToken", moqRequest))
                            .Returns(()=>"Ok");

            // Act
            var actualStatus = dumDumController.RegisterKingdom("moqToken", moqRequest);
            var actualStatusResponse = (actualStatus as ObjectResult).Value as StatusResponse;

            // Assert
            Assert.Equal(expectedStatusResponse.Status , actualStatusResponse.Status);
        }*/

        [Fact]
        public void ListingAllKingdoms_ReturnsCorrectJson()
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
                Location = new DumDum.Models.Entities.Location()
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
        public void KingdomsLeaderboard_ReturnsLeaderboardList()
        {
            var kingdoms = new KingdomsLeaderboardResponse();

            kingdoms.Response = new List<KingdomPoints>()
            {
                new KingdomPoints()
                {
                    Ruler = "Nya",
                    Kingdom = "Hahalkovo",
                    Points = 14
                }
            };

            troopServiceMoq.Setup(x => x.GetKingdomsLeaderboard().Result).Returns(kingdoms);

            var actual = troopServiceMoq.Object.GetKingdomsLeaderboard().Result;

            Assert.IsType<KingdomsLeaderboardResponse>(actual);
            Assert.Equal(kingdoms, actual);
        }
    }
}
