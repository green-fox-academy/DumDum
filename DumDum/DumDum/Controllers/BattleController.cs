using DumDum.Models.JsonEntities;
using DumDum.Models.JsonEntities.Battles;
using DumDum.Services;
using Microsoft.AspNetCore.Mvc;

namespace DumDum.Controllers
{
    public class BattleController : Controller
    {
        public BattleService BattleService { get; set; }
        public BattleController(BattleService battleService)
        {
            BattleService = battleService;
        }
        
        [HttpPost("kingdoms/{defenderKingdomId=int}/battles")]
        public IActionResult Battle([FromHeader] string authorization, [FromRoute] int defenderKingdomId, [FromBody] BattleRequest battleRequest)
        {
            int statusCode = 0;
            var response = BattleService.MakeBattle(authorization, defenderKingdomId, battleRequest, out statusCode);

            if (statusCode == 200)
            {
                return Ok(response);
            }

            if (statusCode == 401)
            {
                return Unauthorized(new ErrorResponse() {Error = "This kingdom does not belong to authenticated player"});
            }

            return BadRequest(new ErrorResponse() {Error = "Invalid credentials"});
        }
        
        [HttpGet("kingdoms/{defenderKingdomId=int}/battles{battleId=int}")]
        public IActionResult BattleResult(string authorization, [FromRoute] int defenderKingdomId, int battleId)
        {
            int statusCode = 0;
            var response = BattleService.GetBattleResult(authorization, defenderKingdomId, battleId, out statusCode);

            if (statusCode == 200)
            {
                return Ok(response);
            }

            if (statusCode == 401)
            {
                return Unauthorized(new ErrorResponse() {Error = "This kingdom does not belong to authenticated player"});
            }

            return BadRequest(new ErrorResponse() {Error = "Invalid credentials"});
        }
       
    }
}