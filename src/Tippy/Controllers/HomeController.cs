using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Tippy.Core;

namespace Tippy.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HomeController : ControllerBase
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<Block> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new Block
            {
                Date = DateTime.Now.AddDays(index),
                Height = index,
            })
            .ToArray();
        }
    }
}
