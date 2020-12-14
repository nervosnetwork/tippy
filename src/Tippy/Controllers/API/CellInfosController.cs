using System;
using Ckb.Rpc;
using Ckb.Types;
using Microsoft.AspNetCore.Mvc;
using Tippy.ApiData;

namespace Tippy.Controllers.API
{
    [Route("api/v1")]
    [ApiController]
    public class CellInfosController : ApiControllerBase
    {
        [HttpGet("cell_output_lock_scripts/{id}")]
        public ActionResult LockScript(string id)
        {
            Client? client = Rpc();
            if (client == null)
            {
                return NoContent();
            }

            var (txHash, index) = ParseId(id);
            var tx = GetTransaction(client, txHash);

            var lockScript = tx.Outputs[index].Lock;

            Result<Script> result = new("lock_script", lockScript);

            return Ok(result);
        }

        [HttpGet("cell_output_type_scripts/{id}")]
        public ActionResult TypeScript(string id)
        {
            Client? client = Rpc();
            if (client == null)
            {
                return NoContent();
            }

            var (txHash, index) = ParseId(id);
            var tx = GetTransaction(client, txHash);

            Script? typeScript = tx.Outputs[index].Type;

            if (typeScript == null)
            {
                return Ok(null);
            }

            Result<Script> result = new("type_script", typeScript);

            return Ok(result);
        }

        [HttpGet("cell_output_data/{id}")]
        public ActionResult Data(string id)
        {
            Client? client = Rpc();
            if (client == null)
            {
                return NoContent();
            }

            var (txHash, index) = ParseId(id);
            var tx = GetTransaction(client, txHash);

            string data = tx.OutputsData[index];

            DataResult dataResult = new()
            {
                Data = data
            };

            Result<DataResult> result = new("data", dataResult);

            return Ok(result);
        }

        private static Transaction GetTransaction(Client client, string txHash)
        {
            var txWithStatus = client.GetTransaction(txHash);
            if (txWithStatus == null)
            {
                throw new Exception("No transaction found!");
            }
            return txWithStatus.Transaction;
        }

        private static (string TxHash, int Index) ParseId(string id)
        {
            string[] info = id.Split(':');
            if (info.Length != 2)
            {
                throw new Exception($"Id: {id} format error!");
            }
            return (info[0], Int32.Parse(info[1]));
        }
    }
}
