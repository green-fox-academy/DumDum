using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using DumDum.Models.Entities;
using Microsoft.AspNetCore.Authorization;

namespace DumDum.Controllers
{
    [Authorize]
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
