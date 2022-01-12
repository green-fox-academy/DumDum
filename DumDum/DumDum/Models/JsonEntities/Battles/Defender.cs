using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using DumDum.Models.Entities;
using Newtonsoft.Json;

namespace DumDum.Models.JsonEntities.Battles
{
    public class Defender
    { 
        public List<TroopsList> TroopsLost { get; set; }
    }
}