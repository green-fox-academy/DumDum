using DumDum.Models.Entities;

namespace DumDum.Models.JsonEntities.Troops
{
    public class TroopsResponse
    {
        public int TroopId { get; set; }
        public string TroopType { get; set; }
        public int Level { get; set; }
        public int HP { get; set; }
        public decimal Attack { get; set; }
        public decimal Defence { get; set; }
        public int StartedAt { get; set; }
        public int FinishedAt { get; set; }

        public TroopsResponse(Troop troop)
        {
            TroopId = troop.TroopId;
            TroopType = troop.TroopType.TroopType;
            Level = troop.Level;
            HP = troop.TroopType.TroopLevel.Level;
            Attack = troop.TroopType.TroopLevel.Attack;
            Defence = troop.TroopType.TroopLevel.Defence;
            StartedAt = troop.StartedAt;
            FinishedAt = troop.FinishedAt;
        }
    }
}
