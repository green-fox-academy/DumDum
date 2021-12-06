using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DumDum.Models.Entities;

namespace DumDum.Controllers
{
    public class DumDumController : Controller
    {
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
    }
}
