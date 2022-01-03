using DumDum.Models.Entities;

namespace DumDum.Interfaces
{
    public interface ITroopLevelRepository : IRepository<TroopLevel>
    {
        int MaximumLevelPossible();
    }
}
