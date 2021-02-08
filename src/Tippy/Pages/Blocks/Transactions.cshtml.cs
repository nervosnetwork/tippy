using System;
using System.Collections.Generic;
using System.Linq;
using Ckb.Rpc;
using Ckb.Types;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Tippy.ApiData;
using Tippy.Ctrl;
using Tippy.Util;
using static Tippy.Helpers.TransactionHelper;

namespace Tippy.Pages.Blocks
{
    public class TransactionsModel : PageModelBase
    {
        public TransactionsModel(Tippy.Core.Data.DbContext context) : base(context)
        {
        }

        public PartialViewResult OnGet(string blockHash, [FromQuery(Name = "page")] int page, [FromQuery(Name = "pageSize")] int pageSize)
        {
            if (ActiveProject == null || !ProcessManager.IsRunning(ActiveProject))
            {
                throw new InvalidOperationException();
            }

            var client = Rpc();

            Block? block = client.GetBlock(blockHash);
            if (block == null)
            {
                throw new ArgumentNullException("Block");
            }

            Meta meta = new()
            {
                Total = (UInt64)block.Transactions.Length,
                PageSize = pageSize,
            };

            int skipCount = (page - 1) * pageSize;
            ArrayResult<TransactionListResult> result = GetTransactions(client, block, skipCount, pageSize, meta);

            return new PartialViewResult
            {
                ViewName = "Transactions/_Transaction",
                ViewData = new ViewDataDictionary<List<TransactionListResult>>(ViewData, result.Data.Select(d => d.Attributes).ToList())
            };
        }

        private ArrayResult<TransactionListResult> GetTransactions(Client client, Block block, int skipCount, int size, Meta? meta = null)
        {
            string prefix = IsMainnet() ? "ckb" : "ckt";
            TransactionListResult[] result = block.Transactions.Skip(skipCount).Take(size).Select((tx, i) =>
            {
                string txHash = tx.Hash ?? "0x";
                bool isCellbase = i == 0 && skipCount == 0;
                UInt64 blockNumber = Hex.HexToUInt64(block.Header.Number);

                TransactionListResult txResult = new()
                {
                    IsCellbase = isCellbase,
                    TransactionHash = txHash,
                    BlockNumber = blockNumber.ToString(),
                    BlockTimestamp = Hex.HexToUInt64(block.Header.Timestamp).ToString(),
                };

                if (isCellbase)
                {
                    var (displayInputs, displayOutputs) = GenerateCellbaseDisplayInfos(client, txHash, tx.Outputs, blockNumber, prefix);
                    txResult.DisplayInputs = displayInputs;
                    txResult.DisplayOutputs = displayOutputs;
                }
                else
                {
                    Input[] inputs = tx.Inputs.Take(10).ToArray();
                    Output[] outputs = tx.Outputs.Take(10).ToArray();
                    Output[] previousOutptus = GetPreviousOutputs(client, inputs);
                    var (displayInputs, displayOutputs) = GenerateNotCellbaseDisplayInfos(inputs, outputs, previousOutptus, prefix, txHash);
                    txResult.DisplayInputs = displayInputs;
                    txResult.DisplayOutputs = displayOutputs;
                }
                return txResult;
            }).ToArray();

            return new ArrayResult<TransactionListResult>("ckb_transactions", result, meta);
        }
    }
}
