using DumDum.Models.Entities;
using DumDum.Models.JsonEntities.Buildings;
using DumDum.Models.JsonEntities.Kingdom;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DumDum.Services
{
    public interface IBuildingService
    {
        Task<List<BuildingList>> GetBuildings(int id);
        Task<KingdomResponse> GetKingdom(int id);
        Task<(BuildingResponse, int)> ListBuildings(string authorization, int kingdomId);
        Task<Building> GetBuildingById(int buildingId);
        Task<(BuildingList, int, string)> LevelUp(int kingdomId, int buildingId, string authorization);
        Task<BuildingLevel> InformationForNextLevel(int levelTypeId, int buildingLevel);
        Task<Kingdom> FindPlayerByKingdomId(int id);
        Task<BuildingType> FindLevelingByBuildingType(string buildingType);
        Task<List<string>> ExistingTypeOfBuildings();
        BuildingList AddBuilding(string building, int id, string authorization, out int statusCode);
        Task<int> GetTownHallLevel(int kingdomId);
        BuildingsLeaderboardResponse GetBuildingsLeaderboard();
    }
}