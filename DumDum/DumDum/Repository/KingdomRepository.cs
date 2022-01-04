using DumDum.Database;
using DumDum.Interfaces;
using DumDum.Models.Entities;
using DumDum.Models.JsonEntities;
using DumDum.Models.JsonEntities.Buildings;
using DumDum.Models.JsonEntities.Kingdom;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace DumDum.Repository
{
    public class KingdomRepository : Repository<Kingdom>, IKingdomRepository
    {
        public KingdomRepository(ApplicationDbContext context) : base(context)
        {
        }

        public Kingdom GetKingdomByName(string kingdomName)
        {
            return DbContext.Kingdoms.Include(k => k.Player).FirstOrDefault(x => x.KingdomName == kingdomName);
        }

        public Kingdom GetKingdomById(int kingdomId)
        {
            return DbContext.Kingdoms.Include(k => k.Player).FirstOrDefault(x => x.KingdomId == kingdomId);
        }

        public KingdomsListResponse GetAllKingdoms()
        {
            KingdomsListResponse response = new KingdomsListResponse();

            response.Kingdoms = DbContext.Kingdoms.Include(k => k.Player).Select(k => new KingdomResponse()
            {
                KingdomId = k.KingdomId,
                KingdomName = k.KingdomName,
                Ruler = k.Player.Username,
                Population = 0,
                Location = new Location()
                {
                    CoordinateX = k.CoordinateX,
                    CoordinateY = k.CoordinateY,
                }
            }).ToList();

            return response;
        }

        public Kingdom FindPlayerByKingdomId(int id)
        {
            var kingdom = DbContext.Kingdoms.Include(p => p.Player)
                .Include(r => r.Resources)
                .FirstOrDefault(k => k.KingdomId == id);
            return kingdom;
        }

        public List<Kingdom> GetAllKingdomsIncludePlayer()
        {
            return DbContext.Kingdoms.Include(k => k.Player).ToList();
        }

        public List<BuildingPoints> GetListBuildingPoints()
        {
            return DbContext.Kingdoms.Select(k => new BuildingPoints()
            {
                Ruler = k.Player.Username,
                Kingdom = k.KingdomName,
                Buildings = DbContext.Buildings.Include(b => b.Kingdom).Where(b => b.KingdomId == k.KingdomId).Count(),
                Points = DbContext.Buildings.Include(b => b.Kingdom).Where(b => b.KingdomId == k.KingdomId).Sum(x => x.Level)
            }).OrderByDescending(t => t.Points).ToList();
        }
    }
}

