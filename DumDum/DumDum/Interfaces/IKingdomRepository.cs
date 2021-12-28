using DumDum.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq;

namespace DumDum.Interfaces
{
    public interface IKingdomRepository : IRepository<Kingdom>
    {
        Kingdom GetKingdomByName(string kingdomName);
    }
}
