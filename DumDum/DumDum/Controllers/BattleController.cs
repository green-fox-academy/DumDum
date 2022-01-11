using DumDum.Models.JsonEntities;
using DumDum.Models.JsonEntities.Battles;
using DumDum.Services;
using Microsoft.AspNetCore.Authorization;
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
        
        [Authorize]
        [HttpPost("kingdoms/{attackerKingdomId=int}/battles")]
        public IActionResult Battle([FromHeader] string authorization, [FromRoute] int attackerKingdomId, [FromBody] BattleRequest battleRequest)
        {
            int statusCode = 0;
            var response = BattleService.MakeBattle(authorization, attackerKingdomId, battleRequest, out statusCode);

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
        
        [Authorize]
        [HttpGet("kingdoms/{attackerKingdomId=int}/battles/{battleId=int}")]
        public IActionResult BattleResult([FromHeader] string authorization, [FromRoute] int attackerKingdomId, int battleId)
        {
            int statusCode = 0;
            var response = BattleService.GetBattleResult(authorization, attackerKingdomId, battleId, out statusCode);

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