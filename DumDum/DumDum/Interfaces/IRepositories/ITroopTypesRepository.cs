using DumDum.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DumDum.Interfaces
{
    public interface ITroopTypesRepository : IRepository<TroopTypes>
    {
        Task<List<string>> PossibleTroopTypesToUpgrade();
        Task<List<string>> PossibleTroopTypes();
    }
}
