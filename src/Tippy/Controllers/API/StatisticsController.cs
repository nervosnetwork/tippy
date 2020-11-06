using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Text.Json;
using Ckb.Rpc;
using Tippy.MockApiData;
using Tippy.ApiData;

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
                    new Dictionary<string, object>()
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
                    new Dictionary<string, object>()
                    {
                        ["tip_block_number"] = response
                    })
            );
            return Ok(result);
        }
    }
}
