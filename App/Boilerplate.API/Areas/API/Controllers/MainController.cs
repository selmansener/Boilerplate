using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;

namespace Boilerplate.API.Areas.API.Controllers
{
    public class MainController : APIBaseController
    {
        private readonly ILogger<MainController> _logger;

        public MainController(ILogger<MainController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;

            return Ok(JsonConvert.SerializeObject(new
            {
                Id = id,
                SomeKey = "SomeValue"
            }));
        }
    }
}