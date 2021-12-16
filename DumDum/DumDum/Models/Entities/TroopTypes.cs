using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DumDum.Models.Entities
{
    public class TroopTypes
    {
        [Key]
        public int TroopTypeId { get; set; }
        public string TroopType { get; set; }
        public List<Troop> Troops { get; set; }
        public TroopLevel TroopLevel { get; set; }

    }
}
