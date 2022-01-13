using DumDum.Models.Entities;
using DumDum.Models.JsonEntities;
using DumDum.Models.JsonEntities.Kingdom;
using DumDum.Models.JsonEntities.Troops;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DumDum.Interfaces
{
    public interface ITroopService
    {
        Task<(GetTroopsResponse, int)> ListTroops(string authorization, int kingdomId);
        Task<List<TroopsResponse>> GetTroops(int kingdomId);
        Task<(string, int)> UpgradeTroops(string authorization, TroopUpgradeRequest troopUpdateReq, int kingdomId);
        Task<(List<TroopsResponse>, int)> CreateTroops(string authorization, TroopCreationRequest troopCreationReq, int kingdomId);
        Troop CreateNewTroop(string troopType, int kingdomId);
        bool DoesAcademyExist(int kingdomId);
        bool IsUpgradeInProgress(int kingdomId, string troopType);
        int GetTroopCreationCost(string troopType, int troopCreationLevel);
        int GetTroopUpdateCost(string troopType);
        int CountTroopsByType(string troopType, int kingdomId);
        int GetTroupTypeIdByTroupTypeName(string troopType);
        int CurrentLevelOfTownhall(int kingdomId);
        int CurrentLevelOTroops(int kingdomId, string troopType);
        Task<TroopsLeaderboardResponse> GetTroopsLeaderboard();
        Task<double> GetAllTroopsConsumptionInKingdom(int kingdomId);
        Task<KingdomsLeaderboardResponse> GetKingdomsLeaderboard();
        public List<Troop> GetActiveTroops();
        public bool IsTroopActive(Troop troop);
    }
}
