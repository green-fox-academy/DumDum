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
        private readonly Mock<IDetailService> detailServiceMoq = new Mock<IDetailService>();
        private readonly Mock<ITimeService> timeServiceMoq = new Mock<ITimeService>();
        private readonly Mock<IUnitOfWork> unitOfWorkMoq = new Mock<IUnitOfWork>();

        /*[Fact]
        public void KingdomsList_ReturnsEmptyObject_WhenNoKingdomPresent()
        {
            // Arrange
            KingdomsListResponse kingdomsEmptyList = new();
            dumDumServiceMoq.Setup(x => x.GetAllKingdoms()).Returns(new Task<KingdomsListResponse>(){Kingdoms = kingdomsEmptyList});
            dumDumController = new DumDumController(dumDumServiceMoq.Object, authenticateServiceMoq.Object,
               detailServiceMoq.Object, unitOfWorkMoq.Object, timeServiceMoq.Object);
            // Act
            var actual = dumDumController.KingdomsList();

            // Assert
            Assert.IsType<OkObjectResult>(actual);
            Assert.Equal(StatusCodes.Status200OK, (actual as ObjectResult).StatusCode);
        }*/

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
    }
}