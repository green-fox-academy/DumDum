using DumDum.Database;
using DumDum.Models.Entities;
using DumDum.Models.JsonEntities;
using DumDum.Models.JsonEntities.Buildings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DumDum.Services
{
    public class BuildingService
    {
        private ApplicationDbContext DbContext { get; set; }
        private AuthenticateService AuthenticateService { get; set; }
        private DumDumService DumDumService { get; set; }
        public BuildingService(ApplicationDbContext dbContex, AuthenticateService authService, DumDumService dumService)
        {
            DbContext = dbContex;
            AuthenticateService = authService;
            DumDumService = dumService;
        }



        public List<BuildingList> GetBuildings(int Id)
        {
            return DbContext.Buildings.Where(b => b.KingdomId == Id).
                Select(b => new BuildingList()
                {
                    BuildingId = b.BuildingId,
                    BuildingType = b.BuildingType,
                    Level = b.Level,
                    StartedAt = b.StartedAt,
                    FinishedAt = b.FinishedAt
                }).ToList();
        }

        public KingdomResponse GetKingdom(int id)
        {
            var kingdom = DumDumService.GetKingdomById(id);
            var player = DumDumService.GetPlayerById(kingdom.PlayerId);
            var locations = DumDumService.AddLocations(kingdom);
            return new KingdomResponse()
            {
                KingdomId = kingdom.KingdomId,
                KingdomName = kingdom.KingdomName,
                Ruler = player.Username,
                Population = 0,
                Location = locations,
            };
        }

        //public BuildingResponse BuildingResponse(string authorization, int kingdomId, out int statusCode)
        //{
            
        //}



    }
}
