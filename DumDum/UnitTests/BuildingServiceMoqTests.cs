using DumDum.Interfaces.IServices;
using DumDum.Models.Entities;
using DumDum.Models.JsonEntities.Buildings;
using DumDum.Services;
using Moq;
using System.Collections.Generic;
using Xunit;

namespace UnitTests
{

    public class BuildingServiceMoqTests
    {
        private Mock<IBuildingService> buildingServiceMoq = new Mock<IBuildingService>();

        [Fact]
        public void BuildingLeaderboard_ReturLeaderboardList_WhenRequestIsCorrect()
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

        [Fact]
        public void LevelUp_ReturnsOk_WhenRequestIsCorrect()
        {
            (BuildingList, int, string) expectedResponse = new
                    (new BuildingList(new Building(), new BuildingLevel()), 200, "Ok");
           

            buildingServiceMoq.Setup(x => x.LevelUp(1, 1, "fakeToken").Result).Returns(expectedResponse);

            var actual = buildingServiceMoq.Object.LevelUp(1, 1, "fakeToken");
            Assert.Equal("Ok", actual.Result.Item3);
        }
    }
}
