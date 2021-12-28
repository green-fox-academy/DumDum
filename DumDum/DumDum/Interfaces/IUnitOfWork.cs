using DumDum.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DumDum.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        ITroopRepository Troops { get; }
        IPlayerRepository Players { get; }
        IKingdomRepository Kingdoms { get; }
        int Complete();
    }
}
