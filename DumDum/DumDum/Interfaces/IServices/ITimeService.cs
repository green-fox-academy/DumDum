using System;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace DumDum.Interfaces
{
    public interface ITimeService
    {
        void OnTimedEvent(Object source, ElapsedEventArgs e);
        void UpdateAllKingdomsEvents();
        void GetKingdomResourcesPerCycle(int kingdomId);
        int GetFoodFromFarms(int kingdomId, int cycles);
        int GetGoldFromMines(int kingdomId, int cycles);
        int FoodAndGoldProduction(int buildingLevel, int buildingTypeId);
        Task StartAsync(CancellationToken cancellationToken);
        Task StopAsync(CancellationToken cancellationToken);
    }
}
