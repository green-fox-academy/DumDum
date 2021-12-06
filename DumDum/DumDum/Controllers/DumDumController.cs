using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using DumDum.Models.Entities;
using DumDum.Models.JsonEntities;
using DumDum.Services;
using Microsoft.AspNetCore.Authorization;


namespace DumDum.Controllers
{
    [Authorize]
    public class DumDumController : Controller
    {
        private DumDumService DumDumService { get; set; }

        public DumDumController(DumDumService dumDumService)
        {
            DumDumService = dumDumService;
        }
        
        [Route("")]
        public IActionResult Index()
        {
            return View ();
        }

        [HttpPost("registration")]
        public IActionResult Register([FromBody] string username, string password, string kingdomname)
        {
            var kingdom = DumDumService.GetKingdomByName(kingdomname);

            if (kingdom is not null)
            {
                var player = DumDumService.Register(username, password, kingdom.KingdomId);
                if (DumDumService.IsValid(username, password) == true)
                {
                    return Ok(new {username = player.Username, kingdomId = kingdom.KingdomId});
                }
                else
                {
                    return BadRequest();
                }
            }
            else
            {
                var newKingdom = DumDumService.CreateKingdom(username);
                var player = DumDumService.Register(username, password, newKingdom.KingdomId);
                return Ok(new {username = player.Username, kingdomId = newKingdom.KingdomId});
            }
        }
    }
}
