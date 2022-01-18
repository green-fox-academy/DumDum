using System.Collections.Generic;
using DumDum.Models.Entities;

namespace DumDum.Interfaces.IRepositories
{
    public interface ITroopsLostRepository : IRepository<TroopsLost>
    {
        TroopsLost TroopToUpdate(int troopId, int winnerId);
        TroopsLost AddTroopsLost(TroopsLost lost);
        List<TroopsLost> GetListOfTroopsLost(int playerId, int battleId);
        void UpdateTroopLost(TroopsLost lost);
    }
}