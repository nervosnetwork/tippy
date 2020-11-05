using Microsoft.AspNetCore.Mvc;
using Tippy.MockApiData;

namespace Tippy.Controllers.API
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class StatisticsController : ControllerBase
    {
        [HttpGet]
        public ActionResult Index()
        {
            return Ok(Loader.JsonFromFile("Statistics"));
        }

        [HttpGet("blockchain_info")]
        public ActionResult BlockChainInfo()
        {
            return Ok(Loader.JsonFromFile("BlockchainInfo"));
        }

        [HttpGet("tip_block_number")]
        public ActionResult TipBlockNumber()
        {
            return Ok(Loader.JsonFromFile("TipBlockNumber"));
        }
    }
}
