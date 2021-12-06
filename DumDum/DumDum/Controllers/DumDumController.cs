using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using DumDum.Models.Entities;
using DumDum.Models.JsonEntities;
using DumDum.Services;




namespace DumDum.Controllers
{
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
        public IActionResult Register([FromBody] PlayerJson playerJson)
        {
            
            var kingdom = DumDumService.GetKingdomByName(playerJson.KingdomName);

            if (kingdom is not null)
            {
                var player = DumDumService.Register(playerJson.Username, playerJson.Password, kingdom.KingdomId);
                if (DumDumService.IsValid(playerJson.Username, playerJson.Password) == true)
                {
                    return Ok(new PlayerJson(){Username = player.Username, KingdomId = kingdom.KingdomId});
                }
                else
                {
                    return BadRequest();
                }
            }
            else
            {
                var newKingdom = DumDumService.CreateKingdom(playerJson.Username);
                var player = DumDumService.Register(playerJson.Username, playerJson.Password, newKingdom.KingdomId);
                return Ok(new {username = player.Username, kingdomId = newKingdom.KingdomId});
            }
        }
    }
}
