using DumDum.Models.Entities;
using DumDum.Models.JsonEntities;
using DumDum.Models.JsonEntities.Kingdom;
using DumDum.Models.JsonEntities.Troops;
using System.Collections.Generic;

namespace DumDum.Interfaces
{
    public interface ITroopService
    {
        GetTroopsResponse ListTroops(string authorization, int kingdomId, out int statusCode);
        List<TroopsResponse> GetTroops(int kingdomId);
        string UpgradeTroops(string authorization, TroopUpgradeRequest troopUpdateReq, int kingdomId, out int statusCode);
        List<TroopsResponse> CreateTroops(string authorization, TroopCreationRequest troopCreationReq, int kingdomId, out int statusCode);
        Troop CreateNewTroop(string troopType, int kingdomId);
        bool DoesAcademyExist(int kingdomId);
        bool IsUpgradeInProgress(int kingdomId, string troopType);
        int GetTroopCreationCost(string troopType, int troopCreationLevel);
        int GetTroopUpdateCost(string troopType);
        int CountTroopsByType(string troopType, int kingdomId);
        int GetTroupTypeIdByTroupTypeName(string troopType);
        int CurrentLevelOfTownhall(int kingdomId);
        int CurrentLevelOTroops(int kingdomId, string troopType);
        TroopsLeaderboardResponse GetTroopsLeaderboard();
        double GetAllTroopsConsumptionInKingdom(int kingdomId);
        KingdomsLeaderboardResponse GetKingdomsLeaderboard();
        public List<Troop> GetActiveTroops();
        public bool IsTroopActive(Troop troop);
    }
}
