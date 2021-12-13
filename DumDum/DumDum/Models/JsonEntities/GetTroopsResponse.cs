using System.Collections.Generic;
using System.Text.Json.Serialization;
using DumDum.Models.Entities;
using Newtonsoft.Json;

namespace DumDum.Models.JsonEntities
{
    public class GetTroopsResponse
    {
        public KingdomResponse Kingdom { get; set; }
        public List<TroopsResponse> Troops { get; set; }
    }
}