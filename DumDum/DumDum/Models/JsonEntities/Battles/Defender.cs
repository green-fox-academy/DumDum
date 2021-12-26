using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using DumDum.Models.Entities;
using Newtonsoft.Json;

namespace DumDum.Models.JsonEntities.Battles
{
    public class Defender
    {
        [JsonIgnore] public int DefenderId { get; set; }
        [JsonIgnore] public string DefenderName { get; set; }
        [JsonIgnore] public Battle Battle { get; set; }
        [JsonIgnore] public int BattleId { get; set; }
        [NotMapped] public List<TroopsList> TroopsLost { get; set; }
    }
}