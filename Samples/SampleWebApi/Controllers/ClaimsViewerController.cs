using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata.Ecma335;

namespace SampleWebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class ClaimsViewerController : ControllerBase
    {
        [HttpGet]
        public Dictionary<string, string> Get()
            => HttpContext.User.Claims.ToDictionary(c => c.Type, c => c.Value);
    }
}