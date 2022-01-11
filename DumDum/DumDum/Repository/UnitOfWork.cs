using DumDum.Database;
using DumDum.Interfaces;


namespace DumDum.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext DbContext;
        public IBuildingRepository Buildings { get; private set; }
        public IKingdomRepository Kingdoms { get; private set; }
        public IPlayerRepository Players { get; private set; }
        public IResourceRepository Resources { get; private set; }
        public ITroopLevelRepository TroopLevels { get; private set; }
        public ITroopRepository Troops { get; private set; }
        public ITroopTypesRepository TroopTypes { get; private set; }
        public IBuildingLevelRepository BuildingLevels { get; private set; }
        public IBuildingTypeRepository BuildingTypes { get; private set; }
        public IBattleRepository Battles { get; private set; }
        public ITroopsLostRepository TroopsLost { get; private set; }

        public UnitOfWork(ApplicationDbContext context)
        {
            DbContext = context;
            Buildings = new BuildingRepository(DbContext);
            BuildingLevels = new BuildingLevelRepository(DbContext);
            BuildingTypes = new BuildingTypeRepository(DbContext);
            Kingdoms = new KingdomRepository(DbContext);
            Players = new PlayerRepository(DbContext);
            Resources = new ResourceRepository(DbContext);
            TroopLevels = new TroopLevelRepository(DbContext);
            Troops = new TroopRepository(DbContext);
            TroopTypes = new TroopTypesRepository(DbContext);
            Battles = new BattleRepository(DbContext);
            TroopsLost = new TroopsLostRepository(DbContext);
        }

        public int Complete()
        {
            return DbContext.SaveChanges();
        }

        public void Dispose()
        {
            DbContext.Dispose();
        }
        
    }
}
