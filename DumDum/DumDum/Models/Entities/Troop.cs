using System;
using System.Collections.Generic;
using System.Linq;

namespace DumDum.Models.Entities
{
    public class Troop
    {
        public int TroopId { get; set; }
        public int Level { get; set; }
        public int StartedAt { get; set; }
        public int FinishedAt { get; set; }
        public int KingdomId { get; set; }
        public Kingdom Kingdom { get; set; }
        public int TroopTypeId { get; set; }
        public TroopTypes TroopType { get; set; }

        public Troop(Troop troop)
        {
            TroopId= troop.TroopId;
            Level = troop.Level;
            StartedAt = troop.StartedAt;
            FinishedAt = troop.FinishedAt;
            KingdomId = troop.KingdomId;
            Kingdom = troop.Kingdom;
            TroopTypeId = troop.TroopTypeId;
            TroopType = troop.TroopType;

        }

        public Troop()
        {
            
        }
    }
}

