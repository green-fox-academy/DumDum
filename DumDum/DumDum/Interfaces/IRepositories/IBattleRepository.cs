using System.Collections.Generic;
using DumDum.Models.Entities;

namespace DumDum.Interfaces.IRepositories
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