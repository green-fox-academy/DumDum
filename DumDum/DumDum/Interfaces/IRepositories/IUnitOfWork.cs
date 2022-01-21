using System;

namespace DumDum.Interfaces.IRepositories
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
        IBuildingLevelRepository BuildingLevels { get; }
        IBuildingTypeRepository BuildingTypes { get; }
        IBattleRepository Battles { get; }
        ITroopsLostRepository TroopsLost { get; }

        int Complete();
    }
}
