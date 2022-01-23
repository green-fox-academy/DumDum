using System.Collections.Generic;
using DumDum.Models.Entities;

namespace DumDum.Interfaces.IRepositories
{
    public interface IBattleRepository : IRepository<Battle>
    {

        decimal MinSpeed(int kingdomId);
        
        decimal SumOfAttackPower(int playerKingdomId);

        List<Troop> GetTroopsByKingdomId(int id);

        decimal GetDefensePower(int kingdomId);

        Battle GetBattleById(int id);

        TroopTypes GetTroopTypeById(int troopTypeId);

        Battle AddBattle(Battle battle);
    }
}