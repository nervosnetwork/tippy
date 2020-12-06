using System;
using System.Collections.Generic;
using Ckb.Rpc;
using Ckb.Types;
using Microsoft.AspNetCore.Mvc;
using Tippy.ApiData;
using Tippy.Core.Models;
using Tippy.Ctrl;
using Tippy.Filters;
using Tippy.Util;
using System.Linq;

namespace Tippy.Controllers.API
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [ServiceFilter(typeof(ActiveProjectFilter))]
    public class TransactionsController : ControllerBase
    {
        private Client? Rpc()
        {
            Project? activeProject = CurrentRunningProject();
            if (activeProject != null)
            {
                return new Client($"http://localhost:{activeProject.NodeRpcPort}");
            }

            return null;
        }

        private Project? CurrentRunningProject()
        {
            Project? activeProject = HttpContext.Items["ActiveProject"] as Project;
            if (activeProject != null && ProcessManager.IsRunning(activeProject))
            {
                return activeProject;
            }
            return null;
        }

        [HttpGet]
        public ActionResult Index([FromQuery(Name = "page")] int? page, [FromQuery(Name = "page_size")] int? pageSize)
        {
            Client? client = Rpc();
            if (client == null)
            {
                return NoContent();
            }

            UInt64 tipBlockNumber = client.GetTipBlockNumber();
            if (page == null || pageSize == null)
            {
                ArrayResult<TransactionResult> txResults = GetTransactions(client, 0, 10, tipBlockNumber);
                return Ok(txResults);
            }

            if (page < 1 || pageSize < 1)
            {
                return NoContent();
            }

            UInt64 skipCount = (UInt64)((page - 1) * pageSize);

            Meta meta = new();
            // TODO: update this
            meta.Total = 1000;
            meta.PageSize = (int)pageSize;
            ArrayResult<TransactionResult> result = GetTransactions(client, skipCount, (int)pageSize, tipBlockNumber, meta);
            return Ok(result);
        }

        private static ArrayResult<TransactionResult> GetTransactions(Client client, ulong skipCount, int size, ulong tipBlockNumber, Meta? meta = null)
        {
            ulong currentSkipCount = 0;
            List<TransactionResult> transactionResults = new();
            ulong currentBlockNumber = tipBlockNumber + 1;
            while (currentBlockNumber > 0)
            {
                currentBlockNumber -= 1;
                Block? block = client.GetBlockByNumber(currentBlockNumber);
                if (block == null)
                {
                    continue;
                }

                int transactionsLength = block.Transactions.Length;
                if (transactionsLength <= 1)
                {
                    continue;
                }

                for (int i = transactionsLength - 1; i > 0; i--)
                {
                    if (currentSkipCount < skipCount)
                    {
                        currentSkipCount += 1;
                        continue;
                    }
                    Transaction tx = block.Transactions[i];

                    UInt64 capacityInvolved = GetInputsCapacities(client, tx);

                    TransactionResult txResult = new()
                    {
                        TransactionHash = tx.Hash ?? "0x",
                        BlockNumber = Hex.HexToUInt64(block.Header.Number).ToString(),
                        BlockTimestamp = Hex.HexToUInt64(block.Header.Timestamp).ToString(),
                        CapacityInvolved = capacityInvolved.ToString(),
                        LiveCellChanges = (tx.Outputs.Length - tx.Inputs.Length).ToString()
                    };
                    transactionResults.Add(txResult);

                    if (transactionResults.Count >= size)
                    {
                        break;
                    }
                }

                if (transactionResults.Count >= size)
                {
                    break;
                }
            }

            ArrayResult<TransactionResult> arrayResult = new("ckb_transaction_list", transactionResults.ToArray(), meta);

            return arrayResult;
        }

        private static UInt64 GetInputsCapacities(Client client, Transaction tx)
        {
            return tx.Inputs
                .Select(i => GetInputCapacity(client, i.PreviousOutput.TxHash, Hex.HexToUInt32(i.PreviousOutput.Index)))
                .Aggregate((sum, cur) => sum + cur);
        }

        private static UInt64 GetInputCapacity(Client client, string txHash, uint index)
        {
            TransactionWithStatus? transactionWithStatus = client.GetTransaction(txHash);
            if (transactionWithStatus == null)
            {
                return 0;
            }
            string capacity = transactionWithStatus.Transaction.Outputs[index].Capacity;
            return Hex.HexToUInt64(capacity);
        }
    }
}
