using DumDum.Interfaces.IServices;
using DumDum.Models.JsonEntities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DumDum.Controllers
{
    public class ResourceController : Controller
    {
        private IResourceService ResourceService { get; set; }

        public ResourceController(IResourceService resourceService)
        {
            ResourceService = resourceService;
        }
       
        [Authorize]
        [HttpGet("kingdoms/{id=int}/resources")]
        public IActionResult Resources([FromRoute] int id, [FromHeader] string authorization)
        {
            var response = ResourceService.ResourceLogic(id, authorization).Result;
            
            if (response.Item2 == 200)
            {
                return Ok(response);
            } 
            if (response.Item2 == 404)
            {
                return NotFound(new ErrorResponse() {Error = "Kingdom not found"});
            }
            return Unauthorized(new ErrorResponse(){Error = "This kingdom does not belong to authenticated player"});
        }
    }
}
