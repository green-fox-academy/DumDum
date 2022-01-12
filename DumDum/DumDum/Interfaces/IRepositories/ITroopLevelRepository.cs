using DumDum.Models.Entities;

namespace DumDum.Interfaces
{
    public interface ITroopLevelRepository : IRepository<TroopLevel>
    {
        int MaximumLevelPossible();
        TroopLevel TroopCreationHigherLevel(string troopType, int troopCreationLevel);
        int GetConsumptionByTroopTypeAndLevel(int troopTypeId, int troopLevel);
    }
}
