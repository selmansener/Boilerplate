using Microsoft.AspNetCore.Mvc;

namespace InvoiceFetcher.API.Areas.Dev.Controllers
{
    [ApiController]
    [Area("dev")]
    [Route("[area]/[controller]")]
    public class DevAPIBaseController : Controller
    {
    }
}
