using DumDum.Interfaces.IServices;
using DumDum.Models.JsonEntities;
using DumDum.Models.JsonEntities.Battles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DumDum.Controllers
{
    public class BattleController : Controller
    {
        private IBattleService BattleService { get; set; }
        public BattleController(IBattleService battleService)
        {
            BattleService = battleService;
            
        }
        
        [Authorize]
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
        
        [Authorize]
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