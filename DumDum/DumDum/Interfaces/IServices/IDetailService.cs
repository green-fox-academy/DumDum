using DumDum.Models.JsonEntities.Authorization;
using DumDum.Models.JsonEntities.Kingdom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DumDum.Interfaces
{
    public interface IDetailService
    {
        KingdomDetailsResponse KingdomInformation(int kingdomId, string authorization, out int statusCode);
    }
}
