using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DumDum.Models.Entities
{
    public class Axeman : Troop
    {
        public Axeman()
        {
            TroopType = "axeman";
            Level = 1;
            HP = 1;
            Attack = 8;
            Defence = 5;
            CarryCap = 30;
            Consumption = 1;
            Speed = 1;
        }
    }
}
