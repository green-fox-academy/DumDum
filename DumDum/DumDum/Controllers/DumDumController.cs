using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DumDum.Models.Entities;
using DumDum.Database;
using DumDum.Services;

namespace DumDum.Controllers
{
    public class DumDumController : Controller
    {
        public DumDumService DumDumService { get; set; }
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
            return Ok();
        }

        [HttpPut("registration")]
        public IActionResult RegisterKingdom([FromBody] int coordinateX, int coordinateY, int kingdomId)
        {
            if (DumDumService.CoordinatesValidation(coordinateX, coordinateY))
            {

            }
        }
    }
}
