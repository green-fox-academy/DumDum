using DumDum.Database;
using DumDum.Interfaces;
using DumDum.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DumDum.Repository
{
    public class PlayerRepository : Repository<Player>, IPlayerRepository
    {
        public PlayerRepository(ApplicationDbContext context) : base(context)
        {
        }

        public Player GetPlayerByUsername(string username)
        {
            return DbContext.Players.Include(p => p.Kingdom).FirstOrDefault(p => p.Username == username);
        }

        public bool AreCredentialsValid(string username, string password)
        {
            return DbContext.Players.Any(p => p.Username != username) &&
                !string.IsNullOrWhiteSpace(username) && password.Length >= 8;
        }

        public Player GetPlayerById(int id)
        {
            return DbContext.Players.Include(p => p.Kingdom).FirstOrDefault(p => p.PlayerId == id);
        }

    }
}
