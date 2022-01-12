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
        
        [HttpPost("kingdoms/{attackerKingdomId=int}/battles")]
        public IActionResult Battle([FromHeader] string authorization, [FromRoute] int attackerKingdomId, [FromBody] BattleRequest battleRequest)
        {
            var response = BattleService.MakeBattle(authorization, attackerKingdomId, battleRequest).Result;

            if (response.Item2 == 200)
            {
                return Ok(response);
            }

            if (response.Item2 == 401)
            {
                return Unauthorized(new ErrorResponse() {Error = "This kingdom does not belong to authenticated player"});
            }

            return BadRequest(new ErrorResponse() {Error = "Invalid credentials"});
        }
        
        [HttpGet("kingdoms/{attackerKingdomId=int}/battles/{battleId=int}")]
        public IActionResult BattleResult([FromHeader] string authorization, [FromRoute] int attackerKingdomId, int battleId)
        {
            var response = BattleService.GetBattleResult(authorization, attackerKingdomId, battleId).Result;

            if (response.Item2 == 200)
            {
                return Ok(response);
            }

            if (response.Item2 == 401)
            {
                return Unauthorized(new ErrorResponse() {Error = "This kingdom does not belong to authenticated player"});
            }

            return BadRequest(new ErrorResponse() {Error = "Invalid credentials"});
        }
       
    }
}