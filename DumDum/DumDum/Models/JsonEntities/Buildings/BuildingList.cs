using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DumDum.Models.Entities;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace DumDum.Models.JsonEntities.Buildings
{
    public class BuildingList
    {
        public int BuildingId { get; set; }
        public string BuildingType { get; set; }
        public int Level { get; set; }
        public int Hp { get; set; }
        public long StartedAt { get; set; }
        public long FinishedAt { get; set; }
        public int Production { get; set; }
        public int Consumption { get; set; }

        public BuildingList(Building building, BuildingLevel nextLevelInfo)
        {
            BuildingId = building.BuildingId;
            BuildingType = building.BuildingType;
            Level = nextLevelInfo.LevelNumber;
            Hp = 1;
            StartedAt = building.StartedAt;
            FinishedAt = building.FinishedAt;
            Production = nextLevelInfo.Production;
            Consumption = nextLevelInfo.Consumption;
        }

        public BuildingList(EntityEntry<Building> build, BuildingType buildingType)
        {
            BuildingId = build.Entity.BuildingId;
            BuildingType = buildingType.BuildingTypeName;
            Level = buildingType.BuildingLevel.LevelNumber;
            Hp = 1;
            StartedAt = build.Entity.StartedAt;
            FinishedAt = build.Entity.FinishedAt;
            Production = buildingType.BuildingLevel.Production;
            Consumption = buildingType.BuildingLevel.Consumption;
        }

        public BuildingList(Building building)
        {
            BuildingId = building.BuildingId;
            BuildingType = building.BuildingType;
            Level = building.Level;
            StartedAt = building.StartedAt;
            FinishedAt = building.FinishedAt;
        }

        public BuildingList(Task<Building> building, BuildingType buildingType)
        {
            BuildingId = building.Result.BuildingId;
            BuildingType = buildingType.BuildingTypeName;
            Level = buildingType.BuildingLevel.LevelNumber;
            Hp = 1;
            StartedAt = building.Result.StartedAt;
            FinishedAt = building.Result.FinishedAt;
            Production = buildingType.BuildingLevel.Production;
            Consumption = buildingType.BuildingLevel.Consumption;
        }
    }
}
