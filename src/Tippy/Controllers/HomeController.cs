using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Tippy.Core;

namespace Tippy.Controllers
{
    [Route("/home")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public ActionResult Index()
        {
            return View();
        }

        /*
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
        }*/
    }
}
