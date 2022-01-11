using DumDum.Models.Entities;
using DumDum.Models.JsonEntities.Buildings;
using DumDum.Models.JsonEntities.Kingdom;
using System.Collections.Generic;

namespace DumDum.Services
{
    public interface IBuildingService
    {
        List<BuildingList> GetBuildings(int id);
        KingdomResponse GetKingdom(int id);
        BuildingResponse ListBuildings(string authorization, int kingdomId, out int statusCode);
        Building GetBuildingById(int buildingId);
        BuildingList LevelUp(int kingdomId, int buildingId, string authorization, out int statusCode, out string errorMessage);
        BuildingLevel InformationForNextLevel(int levelTypeId, int buildingLevel);
        Kingdom FindPlayerByKingdomId(int id);
        BuildingType FindLevelingByBuildingType(string buildingType);
        List<string> ExistingTypeOfBuildings();
        BuildingList AddBuilding(string building, int id, string authorization, out int statusCode);
        int GetTownHallLevel(int kingdomId);
        BuildingsLeaderboardResponse GetBuildingsLeaderboard();
    }
}