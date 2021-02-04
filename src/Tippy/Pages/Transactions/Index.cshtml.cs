using System;
using System.Collections.Generic;
using System.Linq;
using Ckb.Rpc;
using Ckb.Types;
using Tippy.ApiData;
using Tippy.Ctrl;
using Tippy.Util;

namespace Tippy.Pages.Transactions
{
    public class IndexModel : PageModelBase
    {
        public IndexModel(Tippy.Core.Data.DbContext context) : base(context)
        {
        }

        public List<TransactionResult> Result = default!;
        public ArrayResult<TransactionResult> TheResult = default!;
        public UInt64 FromBlock { get; set; } = 0; // Upper, default to tip number
        public UInt64 ToBlock { get; set; } = 0; // Lower

        public void OnGet(int? fromBlock)
        {
            if (ActiveProject == null || !ProcessManager.IsRunning(ActiveProject))
            {
                return;
            }

            Client client = Rpc();
            if (fromBlock != null)
            {
                FromBlock = (UInt64)fromBlock;
            }
            else
            {
                FromBlock = client.GetTipBlockNumber();
            }

            Result = GetTransactions(client);
        }

        private List<TransactionResult> GetTransactions(Client client)
        {
            int minTxsCount = 15;
            List<TransactionResult> result = new();
            UInt64 blockNumber = FromBlock;
            while (blockNumber > 0)
            {
                blockNumber -= 1;
                Block? block = client.GetBlockByNumber(blockNumber);
                if (block == null || block.Transactions.Length == 0)
                {
                    continue;
                }

                foreach (var tx in block.Transactions)
                {
                    UInt64 capacityInvolved = GetInputsCapacities(client, tx);

                    TransactionResult txResult = new()
                    {
                        TransactionHash = tx.Hash ?? "0x",
                        BlockNumber = Hex.HexToUInt64(block.Header.Number).ToString(),
                        BlockTimestamp = Hex.HexToUInt64(block.Header.Timestamp).ToString(),
                        CapacityInvolved = capacityInvolved.ToString(),
                        LiveCellChanges = (tx.Outputs.Length - tx.Inputs.Length).ToString()
                    };
                    result.Add(txResult);
                }

                if (result.Count >= minTxsCount)
                {
                    break;
                }
            }
            ToBlock = blockNumber;
            return result;
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
            string capacity = transactionWithStatus?.Transaction.Outputs[index].Capacity ?? "0x0";
            return Hex.HexToUInt64(capacity);
        }
    }
}
