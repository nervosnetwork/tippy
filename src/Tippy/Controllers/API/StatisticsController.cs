using Microsoft.AspNetCore.Mvc;
using Ckb.Rpc;
using Tippy.MockApiData;
using Tippy.ApiData;
using Tippy.Filters;

using Attributes = System.Collections.Generic.Dictionary<string, object>;
using Tippy.Core.Models;
using Tippy.Ctrl;

namespace Tippy.Controllers.API
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [ServiceFilter(typeof(ActiveProjectFilter))]
    public class StatisticsController : ControllerBase
    {
        private Client? Rpc()
        {
            var activeProject = HttpContext.Items["ActiveProject"] as Project;
            if (activeProject != null && ProcessManager.IsRunning(activeProject))
            {
                return new Client($"http://localhost:{activeProject.NodeRpcPort}");
            }
            return null;
        }

        [HttpGet]
        public ActionResult Index()
        {
            return Ok(Loader.JsonFromFile("Statistics"));
        }

        [HttpGet("blockchain_info")]
        public ActionResult BlockChainInfo()
        {
            var rpc = Rpc();
            if (rpc == null)
            {
                return NoContent();
            }
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
            var rpc = Rpc();
            if (rpc == null)
            {
                return NoContent();
            }
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
