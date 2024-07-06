using Microsoft.AspNetCore.Mvc;

namespace Boilerplate.API.Areas.Dev.Controllers
{
    [ApiController]
    [Area("dev")]
    [Route("[area]/[controller]")]
    public class DevAPIBaseController : Controller
    {
    }
}
