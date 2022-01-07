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

        public async Task<Player> GetPlayerByUsername(string username)
        {
            var player =  DbContext.Players.Include(p => p.Kingdom).FirstOrDefault(p => p.Username == username);
            return await Task.FromResult(player);
        }

        public async Task<bool> AreCredentialsValid(string username, string password)
        {
            var result =  DbContext.Players.Any(p => p.Username != username) &&
                !string.IsNullOrWhiteSpace(username) && password.Length >= 8;
            return await Task.FromResult(result);
        }

        public async Task<Player> GetPlayerById(int id)
        {
            var player = DbContext.Players.Include(p => p.Kingdom).FirstOrDefault(p => p.PlayerId == id);
            return await Task.FromResult(player);
        }

    }
}
