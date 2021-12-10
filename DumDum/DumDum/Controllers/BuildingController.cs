using DumDum.Models.JsonEntities;
using DumDum.Models.JsonEntities.Buildings;
using DumDum.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DumDum.Controllers
{
    public class BuildingController : Controller
    {

        private DumDumService DumDumService { get; set; }
        private BuildingService BuildingService { get; set;}

        public BuildingController(DumDumService dumDumService,BuildingService buildingService)
        {
            DumDumService = dumDumService;
            BuildingService = buildingService;
        }
        
        [AllowAnonymous]
        [HttpGet("kingdoms/{id=int}/buildings")]
        public IActionResult Buildings([FromRoute] int id)
        {
            var kingdom = DumDumService.GetKingdomById(id);
            var player = DumDumService.GetPlayerById(kingdom.PlayerId);
            var locations = DumDumService.AddLocations(kingdom);
            

            if (player.PlayerId != kingdom.PlayerId)
            {
                return Unauthorized(new ErrorResponse { Error = "This kingdom does not belong to authenticated player" });
            }

            return Ok(new BuildingResponse()
            {   
                Kingdom = BuildingService.GetKingdom(id),
                Buildings = BuildingService.GetBuildings(id),
            });
        }
    }
}
