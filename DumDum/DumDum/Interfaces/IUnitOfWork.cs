using System;

namespace DumDum.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        ITroopRepository Troops { get; }
        IPlayerRepository Players { get; }
        IKingdomRepository Kingdoms { get; }
        IResourceRepository Resources { get; }
        IBuildingRepository Buildings { get; }
        ITroopTypesRepository TroopTypes { get; }
        ITroopLevelRepository TroopLevels { get; }

        int Complete();
    }
}
