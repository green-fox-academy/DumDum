using DumDum.Interfaces;
using DumDum.Models.JsonEntities.Kingdom;
using DumDum.Models.JsonEntities.Login;
using DumDum.Models.JsonEntities.Player;
using DumDum.Services;
using Moq;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using Xunit;

namespace TestProjectMock
{
    public class DumDumServiceTests
    {
        private readonly DumDumService sut;
        private readonly Mock<IUnitOfWork> unitOfWorkMoq = new Mock<IUnitOfWork>();
        private readonly Mock<IAuthenticateService> authenticateServiceMoq = new Mock<IAuthenticateService>();
        private int statusCode;

        public DumDumServiceTests()
        {
            sut = new DumDumService(authenticateServiceMoq.Object, unitOfWorkMoq.Object);
        }

        [Fact]
        public void RegisterKingdom_ShouldAssignCoordinates_WhenAuhtorizedAndCoordinatesNotAssignedAlready()
        {
            // Arrange
            var tokenResult = TestLoginReturnToken("Nya", "catcatcat");
            var request = new HttpRequestMessage();
            KingdomRegistrationRequest requestBody = new();
            requestBody.CoordinateY = 88;
            requestBody.CoordinateX = 88;
            requestBody.KingdomId = 1;
            // Act
            var response = sut.RegisterKingdom(tokenResult, requestBody, out statusCode);
            // Assert
        }
    }
}
