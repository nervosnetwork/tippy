using System;
using System.Threading.Tasks;
using Anemonis.AspNetCore.JsonRpc;

namespace Tippy.Api
{
    [JsonRpcRoute("/api")]
    public class TippyRpc : IJsonRpcService
    {
        // TODO:
        //    1. Add tippy api
        //    2. Capture send tx rpc method
        //    3. Pass other to CKB rpc

        [JsonRpcMethod("test1", "p1", "p2")]
        public Task<long> InvokeMethod1Async(long p1, long p2)
        {
            if (p2 == 0L)
            {
                throw new JsonRpcServiceException(100L, "oops");
            }

            return Task.FromResult(p1 / p2);
        }

        [JsonRpcMethod("test2", 0, 1)]
        public Task<long> InvokeMethod2Async(long p1, long p2)
        {
            return Task.FromResult(p1 + p2);
        }
    }
}
