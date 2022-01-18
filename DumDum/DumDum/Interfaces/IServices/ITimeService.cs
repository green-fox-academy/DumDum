using System;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace DumDum.Interfaces.IServices
{
    public interface ITimeService
    {
        Task UpdateAllKingdomsEvents();
        void GetKingdomResourcesPerCycle(int kingdomId);
        Task<int> GetFoodFromFarms(int kingdomId, int cycles);
        Task<int> GetGoldFromMines(int kingdomId, int cycles);
        Task<int> FoodAndGoldProduction(int buildingLevel, int buildingTypeId);
        Task<int> GetConsumptionOfBuildings(int kingdomId);
        Task<int> GetConsumptionOfTroops(int kingdomId);

    }
}
