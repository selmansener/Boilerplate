using Microsoft.AspNetCore.Mvc;

namespace InvoiceFetcher.API.Areas.API.Controllers
{
    [ApiController]
    [Area("api")]
    [Route("[area]/[controller]")]
    public class APIBaseController : Controller
    {
    }
}
