using DumDum.Controllers;
using DumDum.Interfaces;
using DumDum.Models.JsonEntities;
using DumDum.Models.JsonEntities.Kingdom;
using DumDum.Models.JsonEntities.Login;
using DumDum.Models.JsonEntities.Player;
using DumDum.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using Xunit;

namespace TestProjectMock
{
    public class DumDumControllerTests
    {
        private DumDumController dumDumController;
        private readonly Mock<IAuthenticateService> authenticateServiceMoq = new Mock<IAuthenticateService>();
        private Mock<IDumDumService> dumDumServiceMoq = new Mock<IDumDumService>();
        private readonly Mock<IDetailService> detailServiceMoq = new Mock<IDetailService>();

        public DumDumControllerTests()
        {
            dumDumController = new DumDumController(dumDumServiceMoq.Object, authenticateServiceMoq.Object, detailServiceMoq.Object);
        }

        [Fact]
        public void KingdomsList_ReturnsEmptyObject()
        {
            // Arrange
            KingdomsListResponse kingdomsEmptyList = new();
            dumDumServiceMoq.Setup(x => x.GetAllKingdoms()).Returns(kingdomsEmptyList);
            dumDumController = new DumDumController(dumDumServiceMoq.Object, authenticateServiceMoq.Object, detailServiceMoq.Object);

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

            dumDumServiceMoq.Setup(x => x.RegisterKingdom("moqToken", moqRequest, out statusCode))
                            .Returns<StatusCodeResult>(t => { return t.StatusCode(HttpStatusCode.NotFound) });

            // Act
            dumDumController = new DumDumController(dumDumServiceMoq.Object, authenticateServiceMoq.Object, detailServiceMoq.Object);
            var actual = dumDumController.RegisterKingdom("moqToken", moqRequest);

            // Assert
            Assert.Equal(StatusCodes.Status200OK, (actual as ObjectResult).StatusCode);
        }
    }
}
