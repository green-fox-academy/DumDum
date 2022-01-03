using DumDum.Database;
using DumDum.Interfaces;
using DumDum.Models.Entities;
using DumDum.Models.JsonEntities;
using Microsoft.EntityFrameworkCore;
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
    }
}

