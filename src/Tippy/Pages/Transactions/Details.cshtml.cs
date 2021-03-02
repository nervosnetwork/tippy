using System;
using System.Collections.Generic;
using System.Linq;
using Ckb.Rpc;
using Ckb.Types;
using Microsoft.AspNetCore.Mvc;
using Tippy.ApiData;
using Tippy.Util;
using static Tippy.Helpers.TransactionHelper;

namespace Tippy.Pages.Transactions
{
    public class DetailsModel : PageModelBase
    {
        public DetailsModel(Tippy.Core.Data.DbContext context) : base(context)
        {
        }

        public Transaction Transaction = default!;
        public TransactionDetailResult TransactionDetail = default!;
        public List<String> OutputsData = default!;
        public List<Script> OutputLockScripts = default!;
        public List<Script?> OutputTypeScripts = default!;

        public IActionResult OnGet(string? txhash)
        {
            if (txhash == null)
            {
                return NotFound();
            }

            if (ActiveProject == null)
            {
                return NotFound();
            }

            Client client = new($"http://localhost:{ActiveProject.NodeRpcPort}");

            TransactionWithStatus? transactionWithStatus = client.GetTransaction(txhash);
            if (transactionWithStatus == null)
            {
                return NotFound();
            }

            Transaction tx = transactionWithStatus.Transaction;
            OutputsData = tx.Outputs.Select((o, i) => tx.OutputsData[i]).ToList();
            OutputLockScripts = tx.Outputs.Select((o) => o.Lock).ToList();
            OutputTypeScripts = tx.Outputs.Select<Output, Script?>((o) => o.Type).ToList();

            bool isCellbase = tx.Inputs[0].PreviousOutput.TxHash == EmptyHash;
            string prefix = IsMainnet() ? "ckb" : "ckt";

            TransactionDetailResult detail = new()
            {
                IsCellbase = isCellbase,
                Witnesses = tx.Witnesses,
                CellDeps = tx.CellDeps.Select(dep =>
                {
                    return new ApiData.CellDep()
                    {
                        DepType = dep.DepType,
                        OutPoint = new ApiData.OutPoint()
                        {
                            TxHash = dep.OutPoint.TxHash,
                            Index = Hex.HexToUInt32(dep.OutPoint.Index),
                        }
                    };
                }).ToArray(),
                HeaderDeps = tx.HeaderDeps,
                TxStatus = transactionWithStatus.TxStatus.Status,
                TransactionHash = txhash,
                TransactionFee = "0", // "0" for cellbase
                BlockNumber = "",
                Version = "",
                BlockTimestamp = "",
            };

            if (transactionWithStatus.TxStatus.BlockHash != null)
            {
                Block? block = client.GetBlock(transactionWithStatus.TxStatus.BlockHash);
                if (block != null)
                {
                    Header header = block.Header;
                    UInt64 blockNumber = Hex.HexToUInt64(header.Number);

                    detail.BlockNumber = blockNumber.ToString();
                    detail.Version = Hex.HexToUInt32(header.Version).ToString();
                    detail.BlockTimestamp = Hex.HexToUInt64(header.Timestamp).ToString();

                    var (displayInputs, displayOutputs) = GenerateCellbaseDisplayInfos(client, txhash, tx.Outputs, blockNumber, prefix);
                    detail.DisplayInputs = displayInputs;
                    detail.DisplayOutputs = displayOutputs;
                }
            }

            if (!isCellbase)
            {
                var previousOutputs = GetPreviousOutputs(client, tx.Inputs);

                UInt64 transactionFee = previousOutputs.Select(p => Hex.HexToUInt64(p.Capacity)).Aggregate((sum, cur) => sum + cur) -
                    tx.Outputs.Select(o => Hex.HexToUInt64(o.Capacity)).Aggregate((sum, cur) => sum + cur);
                detail.TransactionFee = transactionFee.ToString();

                var (displayInputs, displayOutputs) = GenerateNotCellbaseDisplayInfos(tx.Inputs, tx.Outputs, previousOutputs, prefix, txhash);
                detail.DisplayInputs = displayInputs;
                detail.DisplayOutputs = displayOutputs;
            }

            Transaction = tx;
            TransactionDetail = detail;

            return Page();
        }
    }
}
