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
using IndexerTypes = Ckb.Types.IndexrTypes;
using static Tippy.Helpers.TransactionHelper;
using Ckb.Address;

namespace Tippy.Pages.Addresses
{
    public class TransactionsModel : PageModelBase
    {
        public TransactionsModel(Tippy.Core.Data.DbContext context) : base(context)
        {
        }

        public PartialViewResult OnGet(string address, [FromQuery(Name = "page")] int page, [FromQuery(Name = "pageSize")] int pageSize)
        {
            if (ActiveProject == null || !ProcessManager.IsRunning(ActiveProject))
            {
                throw new InvalidOperationException();
            }

            var client = Rpc();
            var indexerClient = IndexerRpc();

            string prefix = AddressPrefix();
            Script lockScript = Address.ParseAddress(address, prefix);
            IndexerTypes.SearchKey searchKey = new(lockScript, "lock");

            int skipCount = (page - 1) * pageSize;
            string[] allTxHashes = GetTransactionHashes(indexerClient, searchKey);
            int totalCount = allTxHashes.Length;

            string[] txHashes = allTxHashes.Skip(skipCount).Take(pageSize).ToArray();

            return new PartialViewResult
            {
                ViewName = "Transactions/_Transaction",
                ViewData = new ViewDataDictionary<List<TransactionListResult>>(
                    ViewData,
                    GetTransactions(client, txHashes, lockScript))
            };
        }

        private List<TransactionListResult> GetTransactions(Client client, string[] txHashes, Script lockScript)
        {
            string prefix = AddressPrefix();

            UInt64 SumOfOutputCapacities(Output[] outputs) => outputs.Where(o => o.Lock == lockScript).Select(o => Hex.HexToUInt64(o.Capacity)).Aggregate((UInt64)0, (sum, cur) => sum + cur);

            List<TransactionListResult> results = new();
            foreach (string txHash in txHashes)
            {
                TransactionWithStatus? txWithStatus = client.GetTransaction(txHash);
                if (txWithStatus == null)
                {
                    throw new Exception("No TransactionWithStatus found!");
                }
                Transaction tx = txWithStatus.Transaction;

                bool isCellbase = tx.Inputs[0].PreviousOutput.TxHash == EmptyHash;
                TransactionListResult txResult = new()
                {
                    IsCellbase = isCellbase,
                    TransactionHash = txHash,
                    BlockNumber = "",
                    BlockTimestamp = "",
                };

                string? blockHash = txWithStatus.TxStatus.BlockHash;
                if (blockHash == null)
                {
                    throw new Exception("No BlockHash in tx!");
                }
                Header? header = client.GetHeader(blockHash);
                if (header == null)
                {
                    throw new Exception("No Header found!");
                }
                UInt64 blockNumber = Hex.HexToUInt64(header.Number);
                txResult.BlockNumber = blockNumber.ToString();
                txResult.BlockTimestamp = Hex.HexToUInt64(header.Timestamp).ToString();

                if (isCellbase)
                {
                    var (displayInputs, displayOutputs) = GenerateCellbaseDisplayInfos(client, txHash, tx.Outputs, blockNumber, prefix);
                    txResult.DisplayInputs = displayInputs;
                    txResult.DisplayOutputs = displayOutputs;

                    txResult.Income = Hex.HexToUInt64(tx.Outputs[0].Capacity).ToString();
                }
                else
                {
                    Output[] previousOutputs = GetPreviousOutputs(client, tx.Inputs);
                    var (displayInputs, displayOutputs) = GenerateNotCellbaseDisplayInfos(
                        tx.Inputs.Take(10).ToArray(),
                        tx.Outputs.Take(10).ToArray(),
                        previousOutputs.Take(10).ToArray(),
                        prefix,
                        txHash);
                    txResult.DisplayInputs = displayInputs;
                    txResult.DisplayOutputs = displayOutputs;

                    long income = (long)SumOfOutputCapacities(tx.Outputs) - (long)SumOfOutputCapacities(previousOutputs);
                    txResult.Income = income.ToString();
                }

                results.Add(txResult);
            }
            return results;
        }

        private static string[] GetTransactionHashes(IndexerClient indexerClient, IndexerTypes.SearchKey searchKey)
        {
            List<string> txHashes = new();
            string? afterCursor = null;
            int limit = 100;

            while (true)
            {
                var result = indexerClient.GetTransactions(searchKey, order: "desc", limit: limit, afterCursor: afterCursor);
                if (result == null || result.LastCursor == null)
                {
                    break;
                }
                afterCursor = result.LastCursor;

                foreach (var obj in result.Objects)
                {
                    txHashes.Add(obj.TxHash);
                }

                if (result.Objects.Length < limit)
                {
                    break;
                }
            }

            return txHashes.Distinct().ToArray();
        }
    }
}
