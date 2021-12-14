using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DumDum.Models.Entities
{
    public class LastChange
    {
        [Key]
        public int LastChangeId { get; set; } 
        public long RegistrationTime { get; set; }
        public long LastChangeTime { get; set; } 
        public Player Player { get; set; }
        public int PlayerId { get; set; }

    }
}
