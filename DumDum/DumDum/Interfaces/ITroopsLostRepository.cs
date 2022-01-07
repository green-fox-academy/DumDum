using System.Collections.Generic;
using DumDum.Models.Entities;

namespace DumDum.Interfaces
{
    public interface ITroopsLostRepository : IRepository<TroopsLost>
    {
        public TroopsLost TroopToUpdate(int troopId, int winnerId);
        public TroopsLost AddTroopsLost(TroopsLost lost);
        public List<TroopsLost> GetListOfTroopsLost(int playerId, int battleId);
        public void UpdateTroopLost(TroopsLost lost);
    }
}