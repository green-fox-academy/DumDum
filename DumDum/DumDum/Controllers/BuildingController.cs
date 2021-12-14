using DumDum.Models.JsonEntities;
using DumDum.Models.JsonEntities.Buildings;
using DumDum.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DumDum.Controllers
{
    public class BuildingController : Controller
    {
        private BuildingService BuildingService { get; set; }

        public BuildingController(BuildingService buildingService)
        {
            BuildingService = buildingService;
        }

        [Authorize]
        [HttpGet("kingdoms/{id=int}/buildings")]
        public IActionResult Buildings([FromHeader] string authorization, [FromRoute] int Id)
        {
            var response = BuildingService.ListBuildings(authorization, Id, out int statusCode);

            if (statusCode == 401)
            {
                return StatusCode(statusCode, new ErrorResponse { Error = "This kingdom does not belong to authenticated player" });
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
    }
}
