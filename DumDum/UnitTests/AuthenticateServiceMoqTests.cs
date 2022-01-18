using DumDum.Controllers;
using DumDum.Interfaces;
using DumDum.Models.Entities;
using DumDum.Models.JsonEntities;
using DumDum.Models.JsonEntities.Authorization;
using DumDum.Models.JsonEntities.Kingdom;
using DumDum.Models.JsonEntities.Troops;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace UnitTests
{
    public class AuthenticateServiceMoqTests
    {
        private Mock<IAuthenticateService> authenticateServiceMoq = new Mock<IAuthenticateService>();
  
        [Fact]
        public async void RenameKingdom_ShouldReturnChangedName_WhenRequestIsCorrect() 
        {
            var testPlayer = new Player();
            testPlayer.Username = "TestUser";
            testPlayer.KingdomId = 1;
            testPlayer.Kingdom = new Kingdom() { KingdomName = "RenamedKingdom", KingdomId = 1 };
            var testAuthResponse = new AuthResponse() { KingdomId = 1, KingdomName = "Test", Ruler = testPlayer.Username };

            var testKingdomRenameRequest = new KingdomRenameRequest() { KingdomName = "TestKingdom" };

            KingdomRenameResponse expectedResponse = new(testPlayer);

            authenticateServiceMoq.Setup( x => x.
            RenameKingdom(testKingdomRenameRequest, testAuthResponse).Result).Returns(expectedResponse);

            var actual = authenticateServiceMoq.Object.
                RenameKingdom(testKingdomRenameRequest, testAuthResponse);

            Assert.IsType<KingdomRenameResponse>(actual.Result);
            Assert.Equal("RenamedKingdom", actual.Result.KingdomName);
        }
    }
}
