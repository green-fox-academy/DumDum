using DumDum.Database;
using DumDum.Interfaces;
using DumDum.Models.Entities;
using DumDum.Models.JsonEntities.Buildings;
using DumDum.Models.JsonEntities.Kingdom;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DumDum.Repository
{
    public class KingdomRepository : Repository<Kingdom>, IKingdomRepository
    {
        public KingdomRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Kingdom> GetKingdomByName(string kingdomName)
        {
            var kingdom = DbContext.Kingdoms.Include(k => k.Player).FirstOrDefault(x => x.KingdomName == kingdomName);
            return kingdom;
        }

        public async Task<Kingdom> GetKingdomById(int kingdomId)
        {
            return DbContext.Kingdoms.Include(k => k.Player).FirstOrDefault(x => x.KingdomId == kingdomId);
        }

        public async Task<KingdomsListResponse> GetAllKingdoms()
        {
            KingdomsListResponse response = new KingdomsListResponse();

            response.Kingdoms = DbContext.Kingdoms.Include(k => k.Player).Select(k => new KingdomResponse(k)).ToList();
            return response;
        }

        public async Task<Kingdom> AddKingdom(Kingdom kingdom)
        {
            return DbContext.Kingdoms.Add(kingdom).Entity;
        }

        public async Task<Kingdom> FindPlayerByKingdomId(int id)
        {
            var kingdom = DbContext.Kingdoms.Include(p => p.Player)
                .Include(r => r.Resources)
                .FirstOrDefault(k => k.KingdomId == id);
            return kingdom;
        }

        public async Task<List<Kingdom>> GetAllKingdomsIncludePlayer()
        {
            var kingdomList = DbContext.Kingdoms.Include(k => k.Player).Select(k => new Kingdom(k)).ToList();
            return kingdomList;
        }

        public async Task<List<BuildingPoints>> GetListBuildingPoints()
        {
            var point = DbContext.Kingdoms.Include(k => k.Player).Select(k => new BuildingPoints(
                k,
                DbContext.Buildings.Count(b => b.KingdomId == k.KingdomId),
                DbContext.Buildings.Where(b => b.KingdomId == k.KingdomId).Sum(x => x.Level)));
            return point.ToList();
        }
    }
}

