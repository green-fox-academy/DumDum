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

        [AllowAnonymous]
        [HttpPost("registration")]
        public IActionResult Register([FromBody] PlayerJson playerJson)
        {
            var kingdom = DumDumService.GetKingdomByName(playerJson.KingdomName);

            if (kingdom is not null)
            {
                var player = DumDumService.Register(playerJson.Username, playerJson.Password, kingdom);
                if (DumDumService.IsValid(playerJson.Username, playerJson.Password))
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
                if (DumDumService.IsValid(playerJson.Username, playerJson.Password))
                {
                    var newKingdom = DumDumService.CreateKingdom(playerJson.Username);
                    var player = DumDumService.Register(playerJson.Username, playerJson.Password, newKingdom);
                    return Ok(new PlayerJson(){Username = player.Username, KingdomId = newKingdom.KingdomId});
                }
                
                    return BadRequest();
            }
        }

        [AllowAnonymous]
        [HttpPut("registration")]
        public IActionResult RegisterKingdom([FromBody] KingdomJson kingdomJson)
        {
            int statusCode;
            var message = DumDumService.RegisterKingdomLogic(kingdomJson, out statusCode);

            if (statusCode == 200)
            {
                return Ok(new { status = "Ok" });
            }

            return StatusCode(statusCode, new { error = message });
        }
    }
}
