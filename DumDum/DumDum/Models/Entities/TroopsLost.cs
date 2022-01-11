


using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace DumDum.Models.Entities
{
    public class TroopsLost
    {
        [Key]
        [JsonIgnore]public int TroopLostId { get; set; }
        public int Type { get; set; }
        public int Quantity { get; set; }
        [JsonIgnore]public int PlayerId { get; set; }
        [JsonIgnore]public int BattleId { get; set; }
    }
}