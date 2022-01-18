using System.Collections.Generic;
using System.Threading.Tasks;
using DumDum.Models.Entities;

namespace DumDum.Interfaces.IRepositories
{
    public interface ITroopTypesRepository : IRepository<TroopTypes>
    {
        Task<List<string>> PossibleTroopTypesToUpgrade();
        Task<List<string>> PossibleTroopTypes();
    }
}
