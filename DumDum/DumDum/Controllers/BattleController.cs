using Microsoft.AspNetCore.Mvc;

namespace DumDum.Controllers
{
    public class BattleController : Controller
    {
        [HttpPost("kingdoms/{id=int}/battles")]
        public IActionResult Battle([FromRoute] int kingdomId)
        {
            return Ok();
        }
       
    }
}