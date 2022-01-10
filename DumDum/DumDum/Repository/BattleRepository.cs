using System.Collections.Generic;
using DumDum.Database;
using DumDum.Interfaces;
using DumDum.Models.Entities;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace DumDum.Repository
{
    public class BattleRepository : Repository<Battle>, IBattleRepository
    {
        public BattleRepository(ApplicationDbContext context): base(context)
        {
            
        }

        public double MinSpeed(int kingdomId)
        {
            return DbContext.Troops.Include(t => t.TroopType)
                .Where(t => t.KingdomId == kingdomId && t.TroopType.TroopLevel.Level == t.Level)
                .Min(t => t.TroopType.TroopLevel.Speed);
        }

        public Battle AddBattle(Battle battle)
        {
            return DbContext.Battles.Add(battle).Entity;
        }
        public double SumOfAttackPower(int playerKingdomId)
        {
            return DbContext.Troops.Include(t => t.TroopType)
                .Where(t => t.KingdomId == playerKingdomId && t.TroopType.TroopLevel.Level == t.Level)
                .Sum(t => t.TroopType.TroopLevel.Attack);
        }
        
        public List<Troop> GetTroopsByKingdomId(int id)
        {
            if (DbContext.Troops.Where(t => t.KingdomId == id).ToList().Count is 0)
            {
                return null;
            }

            return DbContext.Troops.Where(t => t.KingdomId == id).ToList();
        }

        public double GetDefensePower(int kingdomId)
        {
            return DbContext.Troops.Include(t => t.TroopType)
                .Where(t => t.KingdomId == kingdomId && t.TroopType.TroopLevel.Level == t.Level)
                .Sum(t => t.TroopType.TroopLevel.Defence);
        }

        public Battle GetBattleById(int id)
        {
            return DbContext.Battles.FirstOrDefault(b => b.BattleId == id);
        }

        public TroopTypes GetTroopTypeById(int troopTypeId)
        {
            return DbContext.TroopTypes.FirstOrDefault(t => t.TroopTypeId == troopTypeId);
        }
        
    }
}