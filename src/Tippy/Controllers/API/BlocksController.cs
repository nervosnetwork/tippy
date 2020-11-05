using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tippy.Core;

namespace Tippy.Controllers.API
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class BlocksController : ControllerBase
    {
        [HttpGet]
        public IEnumerable<Block> Index()
        {
            // TODO
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
