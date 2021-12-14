using DumDum.Models.JsonEntities;
using DumDum.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DumDum.Controllers
{
    [Authorize]
    public class TroopController : Controller
    {
        private AuthenticateService AuthenticateService { get; set; }
        private TroopService TroopService { get; set; }
        public TroopController( AuthenticateService authService, TroopService troopService)
        {
            AuthenticateService = authService;
            TroopService = troopService;
        }

        [HttpGet("kingdoms/{kingdomId}/troops")]
        public IActionResult ListTroops([FromHeader] string authorization, [FromRoute] int kingdomId)
        {
            var response = TroopService.ListTroops(authorization, kingdomId, out int statusCode);

            if (statusCode == 200)
            {
                return Ok(response);
            }
            return Unauthorized(new ErrorResponse { Error= "This kingdom does not belong to authenticated player" });
        }

        [HttpPost("kingdoms/{kingdomId}/troops")]
        public IActionResult CreateTroops([FromHeader] string authorization, [FromRoute] int kingdomId, [FromBody] TroopCreationRequest TroopCreationReq )
        {
            var response = TroopService.CreateTroops(authorization, TroopCreationReq, kingdomId, out int statusCode);

            if (statusCode == 200)
            {
                return Ok(response);
            }
            return Unauthorized(new ErrorResponse { Error = "This kingdom does not belong to authenticated player" });
        }
    }
}
