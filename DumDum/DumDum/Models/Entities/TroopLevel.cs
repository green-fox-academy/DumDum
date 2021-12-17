﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DumDum.Models.Entities
{
    public class TroopLevel
    {
        [Key]
        public int TroopLevelId { get; set; }
        public int Level { get; set; }
        public int HP { get; set; }
        public double Attack { get; set; }
        public double Defence { get; set; }
        public double Speed { get; set; }
        public double Consumption { get; set; }
        public int CarryCap { get; set; }
        public int Cost { get; set; }
        public int SpecialSkills { get; set; }
        public int TroopTypeId { get; set; }
        public TroopTypes TroopType { get; set; }

    }
}
