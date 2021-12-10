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
        private LoginService LoginService { get; set; }
        public AuthenticateService AuthenticateService { get; set; }
        private TroopService TroopService { get; set; }
        public TroopController(LoginService service, AuthenticateService auservice, TroopService troopService)
        {
            LoginService = service;
            AuthenticateService = auservice;
            TroopService = troopService;
        }


        [HttpGet("kingdoms/{kingdomId}/troops")]
        public IActionResult ListTroops([FromHeader] string authorization, [FromRoute] int kingdomId)
        {
            int statusCode;
            var response = TroopService.ListTroops(authorization, out statusCode);

            if (statusCode == 200)
            {
                return Ok(response);
            }
            return BadRequest(response.Error);
        
        }
    }
}
