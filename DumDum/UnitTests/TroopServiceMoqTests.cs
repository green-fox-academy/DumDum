using DumDum.Interfaces.IServices;
using DumDum.Models.JsonEntities.Kingdom;
using DumDum.Models.JsonEntities.Troops;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace UnitTests
{
    public class TroopServiceMoqTests
    {
        private Mock<ITroopService> troopServiceMoq = new Mock<ITroopService>();

        [Fact]
        public void TroopsLeaderboard_ReturnsLeaderboardList_WhenRequestIsCorrect()
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
   
        [Fact]
        public void CreateTroops_ReturnsCorrectResponse_WhenRequestIsCorrect() 
        {
            (List<TroopsResponse>,int) expectedResultTuple = new();
            var expectedResultTask = Task.FromResult(expectedResultTuple);
            troopServiceMoq.Setup(x => x.CreateTroops("fakeToken", new TroopCreationRequest(), 1).Result)
                .Returns(expectedResultTask.Result);
            var actual = troopServiceMoq.Object.CreateTroops("fakeToken", new TroopCreationRequest(), 1);

            Assert.Equal(expectedResultTuple.Item1, actual.Result.Item1);
        }

        [Fact]
        public void KingdomsLeaderboard_ReturnsLeaderboardList_WhenRequestIsCorrect()
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
