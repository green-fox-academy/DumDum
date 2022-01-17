using DumDum.Interfaces.IServices;
using DumDum.Models.JsonEntities.Buildings;
using Moq;
using System.Collections.Generic;
using Xunit;

namespace TestProject1
{

    public class BuildingServiceMoqTests
    {
        private Mock<IBuildingService> buildingServiceMoq = new Mock<IBuildingService>();

        [Fact]
        public void BuildingLeaderboard_ReturLeaderboardList()
        {
            var expected = new BuildingsLeaderboardResponse()
            {
                Result = new List<BuildingPoints>()
            {
                new BuildingPoints("Marek", "Pivko", 4, 152)
            }
            };
                       
            buildingServiceMoq.Setup(x => x.GetBuildingsLeaderboard().Result).Returns(expected);

            var actual = buildingServiceMoq.Object.GetBuildingsLeaderboard().Result;

            Assert.Equal(expected, actual);
            Assert.IsType<BuildingsLeaderboardResponse>(actual);
        }
    }
}
