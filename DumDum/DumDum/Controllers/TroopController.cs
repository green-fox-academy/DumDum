using DumDum.Models.JsonEntities;
using DumDum.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DumDum.Controllers
{
    public class TroopController : Controller
    {
        public AuthenticateService AuthenticateService { get; set; }
        private TroopService TroopService { get; set; }
        public TroopController( AuthenticateService auservice, TroopService troopService)
        {
            AuthenticateService = auservice;
            TroopService = troopService;
        }

        [HttpGet("kingdoms/{kingdomId}/troops")]
        public IActionResult ListTroops([FromHeader] string authorization, [FromRoute] int kingdomId)
        {
            int statusCode;
            var response = TroopService.ListTroops(authorization, kingdomId, out statusCode);

            if (statusCode == 200)
            {
                return Ok(response);
            }
            return Unauthorized(new ErrorResponse { Error= "This kingdom does not belong to authenticated player" });
        }
    }
}
