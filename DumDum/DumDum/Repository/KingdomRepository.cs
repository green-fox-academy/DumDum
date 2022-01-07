using DumDum.Database;
using DumDum.Interfaces;
using DumDum.Models.Entities;
using DumDum.Models.JsonEntities;
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
            var kingdom =  DbContext.Kingdoms.Include(k => k.Player).FirstOrDefault(x => x.KingdomName == kingdomName);
            return await Task.FromResult(kingdom);
        }

        public async Task<Kingdom> GetKingdomById(int kingdomId)
        {
            var kingdom = DbContext.Kingdoms.Include(k => k.Player).FirstOrDefault(x => x.KingdomId == kingdomId);
            return await Task.FromResult(kingdom);
        }

        public async Task<KingdomsListResponse> GetAllKingdoms()
        {
            KingdomsListResponse response = new KingdomsListResponse();

            response.Kingdoms = DbContext.Kingdoms.Include(k => k.Player).Select(k => new KingdomResponse(k)).ToList();
            return await Task.FromResult(response);
        }

        public async Task<Kingdom> FindPlayerByKingdomId(int id)
        {
            var kingdom = DbContext.Kingdoms.Include(p => p.Player)
                .Include(r => r.Resources)
                .FirstOrDefault(k => k.KingdomId == id);
            return await Task.FromResult(kingdom);
        }

        public async Task<List<Kingdom>> GetAllKingdomsIncludePlayer()
        {
            var kingdomList =  DbContext.Kingdoms.Include(k => k.Player).ToList();
            return await Task.FromResult(kingdomList);
        }

        public async Task<List<BuildingPoints>> GetListBuildingPoints()
        {
            var point = DbContext.Kingdoms.Include(k => k.Player).Select(k => new BuildingPoints(
                k,
                DbContext.Buildings.Count(b => b.KingdomId == k.KingdomId),
                DbContext.Buildings.Where(b => b.KingdomId == k.KingdomId).Sum(x => x.Level)));
            return await Task.FromResult(point.ToList());
        }
    }
}

