using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tippy.Controllers.API
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class StatisticsController : ControllerBase
    {
        [HttpGet]
        public IEnumerable<int> Index()
        {
            // TODO
            return new [] { 1, 2, 3 };
        }

        [HttpGet("blockchain_info")]
        public IEnumerable<int> BlockChainInfo()
        {
            // TODO
            return new [] { 1, 2, 3 };
        }

        [HttpGet("tip_block_number")]
        public IEnumerable<int> TipBlockNumber()
        {
            // TODO
            return new [] { 1, 2, 3 };
        }
    }
}
