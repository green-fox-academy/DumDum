using DumDum.Database;
using DumDum.Interfaces;
using DumDum.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DumDum.Repository
{
    public class TroopTypesRepository : Repository<TroopTypes>, ITroopTypesRepository
    {
        public TroopTypesRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<List<string>> PossibleTroopTypesToUpgrade()
        {
            var type =  DbContext.TroopTypes.Where(t => t.TroopType.ToLower() != "senator")
                .Select(t => t.TroopType.ToLower()).ToList();
            return type;
        }

        public async Task<List<string>> PossibleTroopTypes()
        {
            var type =  DbContext.TroopTypes.Select(t => t.TroopType.ToLower()).ToList();
            return type;
        }
    }
}
