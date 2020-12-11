using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ckb.Address;
using Ckb.Rpc;
using Ckb.Types;
using Microsoft.AspNetCore.Mvc;
using Tippy.ApiData;
using Tippy.Util;
using IndexerTypes = Ckb.Types.IndexrTypes;

namespace Tippy.Controllers.API
{
    [Route("api/v1/address_transactions")]
    [ApiController]
    public class AddressTransactionsController : ApiControllerBase
    {
        [HttpGet("{address}")]
        public ActionResult Index(string address, [FromQuery(Name = "page")] int page, [FromQuery(Name = "page_size")] int pageSize)
        {
            Client? client = Rpc();
            if (client == null)
            {
                return NoContent();
            }

            IndexerClient? indexerClient = NewIndexerClient();
            if (indexerClient == null)
            {
                return NoContent();
            }

            string prefix = AddressPrefix();
            Script lockScript = Address.ParseAddress(address, prefix);
            IndexerTypes.SearchKey searchKey = new(lockScript, "lock");

            int skipCount = (page - 1) * pageSize;
            string[] allTxHashes = GetTransactionHashes(indexerClient, searchKey);
            int totalCount = allTxHashes.Length;

            string[] txHashes = allTxHashes.Skip(skipCount).Take(pageSize).ToArray();

            Meta meta = new()
            {
                Total = (UInt64)totalCount,
                PageSize = pageSize,
            };

            ArrayResult<BlockTransactionResult> result = GetTransactions(client, txHashes, lockScript, meta);

            return Ok(result);
        }

        private ArrayResult<BlockTransactionResult> GetTransactions(Client client, string[] txHashes, Script lockScript, Meta meta)
        {
            string prefix = AddressPrefix();

            UInt64 SumOfOutputCapacities(Output[] outputs) => outputs.Where(o => o.Lock == lockScript).Select(o => Hex.HexToUInt64(o.Capacity)).Aggregate((UInt64)0, (sum, cur) => sum + cur);

            List<BlockTransactionResult> results = new();
            foreach (string txHash in txHashes)
            {
                TransactionWithStatus? txWithStatus = client.GetTransaction(txHash);
                if (txWithStatus == null)
                {
                    throw new Exception("No TransactionWithStatus found!");
                }
                Transaction tx = txWithStatus.Transaction;

                bool isCellbase = tx.Inputs[0].PreviousOutput.TxHash == TransactionsController.EmptyHash;
                BlockTransactionResult txResult = new()
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
                    var (displayInputs, displayOutputs) = TransactionsController.GenerateCellbaseDisplayInfos(client, txHash, tx.Outputs, blockNumber, prefix);
                    txResult.DisplayInputs = displayInputs;
                    txResult.DisplayOutputs = displayOutputs;

                    txResult.Income = Hex.HexToUInt64(tx.Outputs[0].Capacity).ToString();
                }
                else
                {
                    Output[] previousOutputs = GetPreviousOutputs(client, tx.Inputs);
                    var (displayInputs, displayOutputs) = TransactionsController.GenerateNotCellbaseDisplayInfos(
                        tx.Inputs.Take(10).ToArray(),
                        tx.Outputs.Take(10).ToArray(),
                        previousOutputs.Take(10).ToArray(),
                        prefix);
                    txResult.DisplayInputs = displayInputs;
                    txResult.DisplayOutputs = displayOutputs;

                    UInt64 income = SumOfOutputCapacities(tx.Outputs) - SumOfOutputCapacities(previousOutputs);
                    txResult.Income = income.ToString();
                }

                results.Add(txResult);
            }
            return new ArrayResult<BlockTransactionResult>("ckb_transactions", results.ToArray(), meta);
        }

        private static Output[] GetPreviousOutputs(Client client, Input[] inputs)
        {
            return inputs.Select(input => GetPreviousOutput(client, input)).ToArray();
        }

        private static Output GetPreviousOutput(Client client, Input input)
        {
            TransactionWithStatus? txWithStatus = client.GetTransaction(input.PreviousOutput.TxHash);
            if (txWithStatus == null)
            {
                throw new Exception("");
            }
            return txWithStatus.Transaction.Outputs[Hex.HexToUInt32(input.PreviousOutput.Index)];
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