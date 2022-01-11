using DumDum.Controllers;
using DumDum.Interfaces;
using DumDum.Models;
using DumDum.Models.JsonEntities;
using DumDum.Models.JsonEntities.Kingdom;
using DumDum.Models.JsonEntities.Login;
using DumDum.Models.JsonEntities.Player;
using DumDum.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using Xunit;

namespace TestProjectMock
{
    public class DumDumControllerMoqTests : TestService
    {
        private IAuthenticateService IAuthenticateService { get; set; }
        private DumDumController dumDumController;
        private readonly Mock<IAuthenticateService> authenticateServiceMoq = new Mock<IAuthenticateService>();
        private Mock<IDumDumService> dumDumServiceMoq = new Mock<IDumDumService>();
        private readonly Mock<IDetailService> detailServiceMoq = new Mock<IDetailService>();
        private readonly Mock<ITimeService> timeServiceMoq = new Mock<ITimeService>();
        private readonly Mock<IUnitOfWork> unitOfWorkMoq = new Mock<IUnitOfWork>();

        public DumDumControllerMoqTests()
        {
            dumDumController = new DumDumController(dumDumServiceMoq.Object, authenticateServiceMoq.Object,
                detailServiceMoq.Object, unitOfWorkMoq.Object, timeServiceMoq.Object);
        }

        [Fact]
        public void KingdomsList_ReturnsEmptyObject()
        {
            // Arrange
            KingdomsListResponse kingdomsEmptyList = new();
            dumDumServiceMoq.Setup(x => x.GetAllKingdoms()).Returns(kingdomsEmptyList);

            // Act
            var actual = dumDumController.KingdomsList();

            // Assert
            Assert.IsType<OkObjectResult>(actual);
            Assert.Equal(StatusCodes.Status200OK, (actual as ObjectResult).StatusCode);
        }

        [Fact]
        public void RegisterKingdom_ReturnsOkStatus()
        {
            // Arrange
            int statusCode = 200;
            KingdomRegistrationRequest moqRequest = new();
            moqRequest.CoordinateY = 88;
            moqRequest.CoordinateX = 88;
            moqRequest.KingdomId = 1;
            var expectedStatusResponse = new StatusResponse { Status = "Ok" };

            dumDumServiceMoq.Setup(x => x.RegisterKingdom("moqToken", moqRequest, out statusCode))
                            .Returns(()=>"Ok");

            // Act
            var actualStatus = dumDumController.RegisterKingdom("moqToken", moqRequest);
            var actualStatusResponse = (actualStatus as ObjectResult).Value as StatusResponse;

            // Assert
            Assert.Equal(expectedStatusResponse.Status , actualStatusResponse.Status);
        }
    }
}
