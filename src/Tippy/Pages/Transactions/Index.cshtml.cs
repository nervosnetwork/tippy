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
        public int FromBlock { get; set; } = 0; // Upper, default to tip number
        public int ToBlock { get; set; } = 0; // Lower

        public void OnGet(int? fromBlock, int? toBlock)
        {
            if (ActiveProject == null || !ProcessManager.IsRunning(ActiveProject))
            {
                return;
            }

            bool descending = true; // From higher to lower
            if (fromBlock == null && toBlock == null)
            {
                fromBlock = (int)TipBlockNumber;
            }

            if (fromBlock != null)
            {
                FromBlock = (int)fromBlock;
            }
            else
            {
                descending = false;
                ToBlock = (int)toBlock!;
            }

            Result = GetTransactions(descending);
        }

        private List<TransactionResult> GetTransactions(bool descending)
        {
            Client client = Rpc();

            int minTxsCount = 15;
            List<TransactionResult> result = new();
            int blockNumber = descending ? FromBlock : ToBlock;
            bool condition()
            {
                if (descending)
                {
                    return blockNumber >= 0;
                }
                return blockNumber <= (int)TipBlockNumber;
            }
            void stepper()
            {
                if (descending)
                {
                    ToBlock = blockNumber;
                    blockNumber -= 1;
                }
                else
                {
                    FromBlock = blockNumber;
                    blockNumber += 1;
                }
            }

            while (condition())
            {
                Block? block = client.GetBlockByNumber((UInt64)blockNumber);
                stepper();
                if (block == null || (block.Transactions.Length <= 1 && blockNumber >0)) // Filter out cellbase tx
                {
                    continue;
                }

                var txs = block.Transactions.Skip(1).Select(tx =>
                {
                    UInt64 capacityInvolved = GetInputsCapacities(client, tx);

                    return new TransactionResult()
                    {
                        TransactionHash = tx.Hash ?? "0x",
                        BlockNumber = Hex.HexToUInt64(block.Header.Number).ToString(),
                        BlockTimestamp = Hex.HexToUInt64(block.Header.Timestamp).ToString(),
                        CapacityInvolved = capacityInvolved.ToString(),
                        LiveCellChanges = (tx.Outputs.Length - tx.Inputs.Length).ToString()
                    };
                }).ToList();
                if (descending)
                {
                    result.AddRange(txs);
                }
                else
                {
                    result.InsertRange(0, txs);
                }
                if (result.Count >= minTxsCount)
                {
                    break;
                }
            }
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
