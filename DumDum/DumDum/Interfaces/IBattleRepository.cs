using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using DumDum.Models.Entities;

namespace DumDum.Interfaces
{
    public interface IBattleRepository : IRepository<Battle>
    {

        public double MinSpeed(int kingdomId);
        
        public double SumOfAttackPower(int playerKingdomId);

        public List<Troop> GetTroopsByKingdomId(int id);

        public double GetDefensePower(int kingdomId);

        public Battle GetBattleById(int id);

        public TroopTypes GetTroopTypeById(int troopTypeId);

        public Battle AddBattle(Battle battle);
    }
}