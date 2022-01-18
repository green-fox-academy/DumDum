using System.Collections.Generic;
using System.Linq;
using DumDum.Database;
using DumDum.Interfaces.IRepositories;
using DumDum.Models.Entities;

namespace DumDum.Repository
{
    public class TroopsLostRepository : Repository<TroopsLost>, ITroopsLostRepository
    {
        public TroopsLostRepository(ApplicationDbContext context): base(context)
        {
            
        }
       
        public TroopsLost TroopToUpdate(int troopId, int winnerId)
        {
            return DbContext.TroopsLost.FirstOrDefault(t =>
                t.TroopLostId == troopId && t.PlayerId == winnerId);
        }
        
        public TroopsLost AddTroopsLost(TroopsLost lost)
        {
            return DbContext.TroopsLost.Add(lost).Entity;
        }
        
        public List<TroopsLost> GetListOfTroopsLost(int playerId, int battleId)
        {
            return DbContext.TroopsLost.Where(t => t.PlayerId == playerId && t.BattleId == battleId).ToList();
        }

        public void UpdateTroopLost(TroopsLost lost)
        {
            DbContext.Update(lost);
        }
    }
}