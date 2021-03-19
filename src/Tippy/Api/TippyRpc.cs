using System;
using Microsoft.AspNetCore.Mvc;

namespace Tippy.Api
{
    [Route("api")]
    [ApiController]
    public class TippyRpc : ControllerBase
    {
        [HttpPost]
        public ActionResult Index()
        {
            var result = new
            {
                jsonrpc = "2.0",
                result = "0x2cb4",
                id = 2
            };
            return Ok(result);
        }
        // TODO:
        //    1. Add tippy api
        //    2. Capture send tx rpc method
        //    3. Pass other to CKB rpc
    }
}
