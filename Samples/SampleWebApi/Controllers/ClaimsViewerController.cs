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
        public List<string> Get()
            => HttpContext.User.Claims.Select(c => $"{c.Type} : {c.Value}").ToList();
    }
}