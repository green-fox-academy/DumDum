using DumDum.Interfaces;
using DumDum.Models.JsonEntities.Troops;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace TestProject1
{
    public class TroopServiceMoqTests
    {
        private Mock<ITroopService> troopServiceMoq = new Mock<ITroopService>();

        [Fact]
        public void TroopsLeaderboardReturLeaderboardListMoq()
        {
            var troops = new TroopsLeaderboardResponse()
            {
                Result = new List<TroopsPoint>() {new TroopsPoint("Nya", "Hahalkovo", 5, 11)}
            };

            troopServiceMoq.Setup(x => x.GetTroopsLeaderboard().Result).Returns(troops);

            var actual =troopServiceMoq.Object.GetTroopsLeaderboard().Result;

            Assert.IsType<TroopsLeaderboardResponse>(actual);
            Assert.Equal(troops, actual);
                        
        }
    }
}
