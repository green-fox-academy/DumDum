using System.Threading.Tasks;
using DumDum.Models.JsonEntities;
using DumDum.Models.JsonEntities.Buildings;
using DumDum.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DumDum.Controllers
{
    public class BuildingController : Controller
    {

        private DumDumService DumDumService { get; set; }
        private BuildingService BuildingService { get; set; }

        public BuildingController(DumDumService dumDumService, BuildingService buildingService)
        {
            DumDumService = dumDumService;
            BuildingService = buildingService;
        }

        [Authorize]
        [HttpGet("kingdoms/{id=int}/buildings")]
        public IActionResult Buildings([FromHeader] string authorization, [FromRoute] int Id)
        {
            int statusCode;
            var response = BuildingService.ListBuildings(authorization, Id).Result;

            if (response.Item2 == 401)
            {
                return StatusCode(response.Item2, new ErrorResponse { Error = "This kingdom does not belong to authenticated player" });
            }
            return Ok(response.Item1);
        }
        
        [Authorize]
        [HttpPut("kingdoms/{kingdomId=int}/buildings/{buildingId=int}")]
        public IActionResult UpgradeBuildings([FromHeader] string authorization, [FromRoute] int kingdomId, int buildingId)
        {
            var response = BuildingService.LevelUp(kingdomId, buildingId,authorization).Result;

            if (response.Item2 == 401)
            {
                return StatusCode(response.Item2, new ErrorResponse { Error = response.Item3});
            } 
            if(response.Item2 == 400)
            {
                return StatusCode(response.Item2, new ErrorResponse { Error = response.Item3});
            }
            return Ok(response.Item1);
        }
        
        [Authorize]
        [HttpPost("kingdoms/{id=int}/buildings")]
        public IActionResult AddBuilding([FromHeader] string authorization, [FromRoute] int id, [FromBody] BuildingAddRequest type)
        {
            var response = BuildingService.AddBuilding(type.Type, id, authorization).Result;
            if (response.Item2 == 400)
            {
                return StatusCode(response.Item2, new ErrorResponse {Error = "You don't have enough gold to build that!"});
            }

            if (response.Item2 == 406)
            {
                return StatusCode(response.Item2, new ErrorResponse {Error = "Type is required."});
            }

            if (response.Item2 == 401)
            {
                return StatusCode(response.Item2, new ErrorResponse {Error = "This kingdom does not belong to authenticated player"});
            }
            return Ok(response.Item1);
        }

        [HttpGet("leaderboards/buildings")]
        public IActionResult BuildingsLeaderboard()
        {
            var buildingsLeaderboard = BuildingService.GetBuildingsLeaderboard().Result;

            return Ok(buildingsLeaderboard);
        }

    }
}
