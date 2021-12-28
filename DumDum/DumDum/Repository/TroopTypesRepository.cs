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
    }
}
