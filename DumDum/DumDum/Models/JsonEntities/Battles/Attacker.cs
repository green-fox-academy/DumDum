using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using DumDum.Models.Entities;
using Newtonsoft.Json;

namespace DumDum.Models.JsonEntities.Battles
{
    public class Attacker
    {
        [JsonIgnore] public int AttackerId { get; set; }
        [JsonIgnore] public string AttackerName { get; set; }
        [JsonIgnore] public Battle Battle { get; set; }
        [JsonIgnore] public int BattleId { get; set; }

        [NotMapped] public ResourcesStolen ResourcesStolen { get; set; }
        [NotMapped] public List<TroopsList> TroopsLost { get; set; }
    }
}