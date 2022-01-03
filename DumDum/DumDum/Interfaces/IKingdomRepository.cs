using DumDum.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq;
using DumDum.Models.JsonEntities;

namespace DumDum.Interfaces
{
    public interface IKingdomRepository : IRepository<Kingdom>
    {
        Kingdom GetKingdomByName(string kingdomName);
        Kingdom GetKingdomById(int kingdomId);
        KingdomsListResponse GetAllKingdoms();

    }
}
