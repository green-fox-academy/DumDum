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
        public async void RenameKingdom_ShouldReturnChangedName() 
        {
            var testPlayer = new Player();
            testPlayer.Username = "TestUser";
            testPlayer.KingdomId = 1;
            testPlayer.Kingdom = new Kingdom() { KingdomName = "TestKingdom", KingdomId = 1 };
            var testAuthResponse = new AuthResponse() { KingdomId = 1, KingdomName = "Test", Ruler = testPlayer.Username };

            var testKingdomRenameRequest = new KingdomRenameRequest() { KingdomName = "TestKingdom" };

            KingdomRenameResponse expectedResponse = new(testPlayer);
           // var TaskToReturn = Task.FromResult(expectedResponse);

            authenticateServiceMoq.Setup( x => x.
            RenameKingdom(testKingdomRenameRequest, testAuthResponse)).Returns(Task.FromResult(expectedResponse));

            var actual = await authenticateServiceMoq.Object.
                RenameKingdom(new KingdomRenameRequest() { KingdomName="TestKingdom"}, testAuthResponse);

            Assert.IsType<KingdomRenameResponse>(actual);
        }
    }
}
