using System.Collections.Generic;
using System.Text.Json.Serialization;
using DumDum.Models.Entities;
using Newtonsoft.Json;

namespace DumDum.Models.JsonEntities
{
    public class GetTroopsResponse
    {
        public int KingdomId { get; set; }
        public string KingdomName { get; set; }
        public string Ruler { get; set; }
        public int Population { get; set; }
        public Location Location { get; set; }
        public List<TroopsResponse> Troops { get; set; }
        
        [JsonProperty("Error", NullValueHandling = NullValueHandling.Ignore)]
        public string Error { get; set; }
    }
}