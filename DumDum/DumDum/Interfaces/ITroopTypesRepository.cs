using DumDum.Models.Entities;
using System.Collections.Generic;

namespace DumDum.Interfaces
{
    public interface ITroopTypesRepository : IRepository<TroopTypes>
    {
        List<string> PossibleTroopTypesToUpgrade();
    }
}
