using System;
using System.Collections.Generic;
using System.Linq;
using Ckb.Rpc;
using Ckb.Types;
using Microsoft.AspNetCore.Mvc;
using Tippy.ApiData;
using Tippy.Core.Models;
using Tippy.Ctrl;
using Tippy.Filters;
using Tippy.Util;

namespace Tippy.Pages.Transactions
{
    [ServiceFilter(typeof(ActiveProjectFilter))]
    public class IndexModel : PageModelBase
    {
        public IndexModel(Tippy.Core.Data.DbContext context) : base(context)
        {
        }

        public Project? ActiveProject { get; set; }
        public ArrayResult<TransactionResult> Result = default!;

        public void OnGet()
        {
            if (HttpContext.Items["ActiveProject"] is Project activeProject && ProcessManager.IsRunning(activeProject))
            {
                ActiveProject = activeProject;
            }
            else
            {
                return;
            }

            Client client = Rpc();

            UInt64 page = 1;
            UInt64 pageSize = 20;
            UInt64 tipBlockNumber = client.GetTipBlockNumber();
            UInt64 skipCount = (UInt64)((page - 1) * pageSize);

            Meta meta = new()
            {
                // TODO: update this
                Total = 1000,
                PageSize = (int)pageSize
            };
            Result = GetTransactions(client, skipCount, (int)pageSize, tipBlockNumber, meta);
        }

        private Client Rpc() => new Client($"http://localhost:{ActiveProject!.NodeRpcPort}");

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
