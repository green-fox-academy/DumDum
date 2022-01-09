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
    public class DumDumControllerTests : TestService
    {
        private IAuthenticateService IAuthenticateService { get; set; }
        [Fact]
        public void HttpPutRegistration_ReturnsStatusOkAndCorrectResponse()
        {
            //arrange
            //var unitOfWork = GetContextWithoutData();
            IOptions<AppSettings> AppSettings = Options.Create<AppSettings>(new AppSettings() { Key = "This is my sample key" });
            //var testPlayerRequest = new PlayerRequest { KingdomName = "TestKingdom", Password = "TestPassword", Username = "TestUser" };
            //dumDumService.RegisterPlayerLogic(testPlayerRequest, out _);
            //var LoginService = new LoginService(AppSettings, dumDumService, unitOfWork);
            //var token = LoginService.
            //    Login(new LoginRequest { Username = testPlayerRequest.Username, Password = testPlayerRequest.Password }, out _);

            var request = new HttpRequestMessage();
            StatusResponse expectedStatusResult = new();
            expectedStatusResult.Status = "Ok";
            HttpStatusCode expectedStatusCode = HttpStatusCode.OK;
            string token = TestLoginReturnTokenPlayerInMemoryDB(out IUnitOfWork unitOfWork);
            var AuthenticateService = new AuthenticateService(AppSettings, unitOfWork);
            var dumDumService = new DumDumService(AuthenticateService, unitOfWork);

            KingdomRegistrationRequest requestBody = new();
            requestBody.CoordinateY = 88;
            requestBody.CoordinateX = 88;
            requestBody.KingdomId = 1;
            string requestBodyContent = JsonConvert.SerializeObject(requestBody);
            StringContent requestContent = new(requestBodyContent, Encoding.UTF8, "application/json");
            request.RequestUri = new Uri("http://localhost:5000/registration");
            request.Method = HttpMethod.Put;
            request.Content = requestContent;
            request.Headers.Add("authorization", $"bearer {token}");

            //act
            //  var response = HttpClient.SendAsync(request).Result;
            var response = dumDumService.RegisterKingdom(token, requestBody, out int StatusCode);
            //  string responseBodyContent = response.Content.ReadAsStringAsync().Result;
            //  StatusResponse responseData = JsonConvert.DeserializeObject<StatusResponse>(responseBodyContent);


            //assert
            // Assert.Equal(expectedStatusCode, response.StatusCode);
            // Assert.Equal(expectedStatusResult.Status, responseData.Status);
            Assert.Equal(response, "Ok");
        }


        [Fact]
        public void DumDumService_GetPlayerById_ReturnsPlayer()
        {
            //arrange
            var UnitOfWork = GetContextWithoutData();
            var DumDumService = new DumDumService(IAuthenticateService, UnitOfWork);
            var testPlayer = DumDumService.
                RegisterPlayerLogic(new PlayerRequest { KingdomName = "testk", Password = "123456789", Username = "user" }, out int StatusCode);
            UnitOfWork.Kingdoms.Add(DumDumService.GetKingdomById(testPlayer.KingdomId));
            UnitOfWork.Players.Add(DumDumService.GetPlayerByUsername(testPlayer.Username));
            //UnitOfWork.Complete();

            //act
            var actualPlayer = DumDumService.GetPlayerById(1);

            //assert
            Assert.Equal(testPlayer.Username, actualPlayer.Username);

        }
        //        private DumDumController dumDumController;
        //        private readonly Mock<IAuthenticateService> authenticateServiceMoq = new Mock<IAuthenticateService>();
        //        private Mock<IDumDumService> dumDumServiceMoq = new Mock<IDumDumService>();
        //        private readonly Mock<IDetailService> detailServiceMoq = new Mock<IDetailService>();
        //        private readonly Mock<ITimeService> timeServiceMoq = new Mock<ITimeService>();
        //        public DumDumControllerTests()
        //        {
        //            dumDumController = new DumDumController(dumDumServiceMoq.Object, authenticateServiceMoq.Object, detailServiceMoq.Object);
        //        }

        //        [Fact]
        //        public void KingdomsList_ReturnsEmptyObject()
        //        {
        //            // Arrange
        //            KingdomsListResponse kingdomsEmptyList = new();
        //            dumDumServiceMoq.Setup(x => x.GetAllKingdoms()).Returns(kingdomsEmptyList);
        //            dumDumController = new DumDumController(dumDumServiceMoq.Object, authenticateServiceMoq.Object, detailServiceMoq.Object);

        //            // Act
        //            var actual = dumDumController.KingdomsList();

        //            // Assert
        //            Assert.IsType<OkObjectResult>(actual);
        //            Assert.Equal(StatusCodes.Status200OK, (actual as ObjectResult).StatusCode);
        //        }

        //        [Fact]
        //        public void RegisterKingdom_ReturnsOkStatus()
        //        {
        //            // Arrange
        //            int statusCode = 200;
        //            KingdomRegistrationRequest moqRequest = new();
        //            moqRequest.CoordinateY = 88;
        //            moqRequest.CoordinateX = 88;
        //            moqRequest.KingdomId = 1;

        //            dumDumServiceMoq.Setup(x => x.RegisterKingdom("moqToken", moqRequest, out statusCode))
        //                            .Returns<StatusCodeResult>(t => { return t.StatusCode(HttpStatusCode.NotFound) });

        //            // Act
        //            dumDumController = new DumDumController(dumDumServiceMoq.Object, authenticateServiceMoq.Object, detailServiceMoq.Object);
        //            var actual = dumDumController.RegisterKingdom("moqToken", moqRequest);

        //            // Assert
        //            Assert.Equal(StatusCodes.Status200OK, (actual as ObjectResult).StatusCode);
        //        }
    }
}
