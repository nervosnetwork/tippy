using Microsoft.AspNetCore.Mvc;
using Ckb.Rpc;
using Tippy.MockApiData;
using Tippy.ApiData;

using Attributes = System.Collections.Generic.Dictionary<string, object>;

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
            var rpc = new Client("http://localhost:8114");
            var response = rpc.GetBlockchainInfo();
            var result = new Result(
                new Data("statistic",
                    new Attributes()
                    {
                        ["blockchain_info"] = response
                    })
            );
            return Ok(result);
        }

        [HttpGet("tip_block_number")]
        public ActionResult TipBlockNumber()
        {
            var rpc = new Client("http://localhost:8114");
            var response = rpc.GetTipBlockNumber();
            var result = new Result(
                new Data("statistic_info",
                    new Attributes()
                    {
                        ["tip_block_number"] = response
                    })
            );
            return Ok(result);
        }
    }
}
