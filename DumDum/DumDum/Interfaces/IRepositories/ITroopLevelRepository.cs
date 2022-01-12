using System.Threading.Tasks;
using DumDum.Models.Entities;

namespace DumDum.Interfaces
{
    public interface ITroopLevelRepository : IRepository<TroopLevel>
    {
        Task<int> MaximumLevelPossible();
        Task<TroopLevel> TroopCreationHigherLevel(string troopType, int troopCreationLevel);
    }
}
