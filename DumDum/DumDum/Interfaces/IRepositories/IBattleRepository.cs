using DumDum.Models.Entities;
using System.Collections.Generic;

namespace DumDum.Interfaces
{
    public interface IBattleRepository : IRepository<Battle>
    {

        double MinSpeed(int kingdomId);
        
        double SumOfAttackPower(int playerKingdomId);

        List<Troop> GetTroopsByKingdomId(int id);

        double GetDefensePower(int kingdomId);

        Battle GetBattleById(int id);

        TroopTypes GetTroopTypeById(int troopTypeId);

        Battle AddBattle(Battle battle);
    }
}