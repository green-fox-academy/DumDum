using DumDum.Models.JsonEntities;
using DumDum.Models.JsonEntities.Troops;
using DumDum.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DumDum.Controllers
{
    public class TroopController : Controller
    {
        private TroopService TroopService { get; set; }
        public TroopController(TroopService troopService)
        {
            TroopService = troopService;
        }

        [Authorize]
        [HttpGet("kingdoms/{kingdomId}/troops")]
        public IActionResult ListTroops([FromHeader] string authorization, [FromRoute] int kingdomId)
        {
            var response = TroopService.ListTroops(authorization, kingdomId, out int statusCode);

            if (statusCode == 200)
            {
                return Ok(response);
            }
            return Unauthorized(new ErrorResponse { Error = "This kingdom does not belong to authenticated player" });
        }

        [Authorize]
        [HttpPost("kingdoms/{kingdomId}/troops")]
        public IActionResult CreateTroops([FromHeader] string authorization, [FromRoute] int kingdomId, [FromBody] TroopCreationRequest TroopCreationReq)
        {
            var response = TroopService.CreateTroops(authorization, TroopCreationReq, kingdomId, out int statusCode);

            if (statusCode == 200)
            {
                return Ok(response);
            }
            if (statusCode == 400)
            {
                return BadRequest(new ErrorResponse { Error = "You don't have enough gold to train all these units!" });
            }
            if (statusCode == 404)
            {
                return BadRequest(new ErrorResponse { Error = "Request was not done correctly!" });
            }
            return Unauthorized(new ErrorResponse { Error = "This kingdom does not belong to authenticated player" });
        }

        [Authorize]
        [HttpPut("kingdoms/{kingdomId}/troops")]
        public IActionResult UpgradeTroops([FromHeader] string authorization, [FromRoute] int kingdomId, [FromBody] TroopUpgradeRequest TroopUpdateReq)
        {
            var response = TroopService.UpgradeTroops(authorization, TroopUpdateReq, kingdomId, out int statusCode);

            if (statusCode is 200)
            {
                return Ok(new StatusResponse() { Status = response });
            }
            if (statusCode is 400 or 402 or 403 or 404 or 406 or 407)
            {
                return BadRequest(new ErrorResponse { Error = response });
            }
            return Unauthorized(new ErrorResponse { Error = response });
        }

        [HttpGet("leaderboards/troops")]
        public IActionResult TroopsLeaderboard()
        {
            var troopsLeaderboard = TroopService.GetTroopsLeaderboard();

            return Ok(troopsLeaderboard);
        }

        [HttpGet("leaderboards/kingdoms")]
        public IActionResult KingdomsLeaderboard()
        {
            var kingdomsLeaderboard = TroopService.GetKingdomsLeaderboard();

            return Ok(kingdomsLeaderboard);
        }
    }
}
