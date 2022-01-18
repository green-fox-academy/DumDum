using System;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace DumDum.Interfaces.IServices
{
    public interface ITimeService
    {
        Task OnTimedEvent(Object source, ElapsedEventArgs e);
        Task UpdateAllKingdomsEvents();
        Task GetKingdomResourcesPerCycle(int kingdomId);
        Task<int> GetFoodFromFarms(int kingdomId, int cycles);
        Task<int> GetGoldFromMines(int kingdomId, int cycles);
        Task<int> HomMuchFoodOneFarmProduce(int lvl);
        Task<int> HomMuchGoldOneMineProduce(int lvl);
        Task StartAsync(CancellationToken cancellationToken);
        Task StopAsync(CancellationToken cancellationToken);
    }
}
