using DumDum.Interfaces;
using DumDum.Models.JsonEntities;
using DumDum.Models.JsonEntities.Buildings;
using DumDum.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DumDum.Controllers
{
    public class BuildingController : Controller
    {

        private IDumDumService DumDumService { get; set; }
        private IBuildingService BuildingService { get; set; }
        private ITimeService TimeService { get; set; }

        public BuildingController(IDumDumService dumDumService, IBuildingService buildingService, ITimeService timeService)
        {
            DumDumService = dumDumService;
            BuildingService = buildingService;
            TimeService = timeService;
        }

        [Authorize]
        [HttpGet("kingdoms/{kingdomId=int}/buildings")]
        public IActionResult Buildings([FromHeader] string authorization, [FromRoute] int kingdomId)
        {
            int statusCode;
            var response = BuildingService.ListBuildings(authorization, kingdomId, out statusCode);


            if (statusCode == 401)
            {
                return StatusCode(statusCode, new ErrorResponse { Error = "This kingdom does not belong to authenticated player" });
            }
            return Ok(response);
        }
        
        [Authorize]
        [HttpPut("kingdoms/{kingdomId=int}/buildings/{buildingId=int}")]
        public IActionResult UpgradeBuildings([FromHeader] string authorization, [FromRoute] int kingdomId, int buildingId)
        {
            var response = BuildingService.LevelUp(kingdomId, buildingId,authorization, out int statusCode, out string errormessage);

            if (statusCode == 401)
            {
                return StatusCode(statusCode, new ErrorResponse { Error = errormessage });
            } 
            if(statusCode == 400)
            {
                return StatusCode(statusCode, new ErrorResponse { Error = errormessage });
            }
            return Ok(response);
        }
        
        [Authorize]
        [HttpPost("kingdoms/{id=int}/buildings")]
        public IActionResult AddBuilding([FromHeader] string authorization, [FromRoute] int id, [FromBody] BuildingAddRequest type)
        {
            var response = BuildingService.AddBuilding(type.Type, id, authorization, out int statusCode);
            if (statusCode == 400)
            {
                return StatusCode(statusCode, new ErrorResponse {Error = "You don't have enough gold to build that!"});
            }

            if (statusCode == 406)
            {
                return StatusCode(statusCode, new ErrorResponse {Error = "Type is required."});
            }

            if (statusCode == 401)
            {
                return StatusCode(statusCode, new ErrorResponse {Error = "This kingdom does not belong to authenticated player"});
            }
            return Ok(response);
        }

        [HttpGet("leaderboards/buildings")]
        public IActionResult BuildingsLeaderboard()
        {
            var buildingsLeaderboard = BuildingService.GetBuildingsLeaderboard();

            return Ok(buildingsLeaderboard);
        }

    }
}
