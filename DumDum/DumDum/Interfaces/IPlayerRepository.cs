using DumDum.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq;

namespace DumDum.Interfaces
{
    public interface IPlayerRepository : IRepository<Player>
    {
        Player GetPlayerByUsername(string username);
        bool AreCredentialsValid(string username, string password);
        Player GetPlayerById(int id);
    }
}
