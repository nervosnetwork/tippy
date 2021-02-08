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

namespace Tippy.Pages.Blocks
{
    public class TransactionsModel : PageModelBase
    {
        public TransactionsModel(Tippy.Core.Data.DbContext context) : base(context)
        {
        }

        public PartialViewResult OnGet(string blockHash, [FromQuery(Name = "page")] int? page)
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

            int pageSize = 100;

            Meta meta = new()
            {
                Total = (UInt64)block.Transactions.Length,
                PageSize = pageSize,
            };

            // TODO: real pagination?
            int skipCount = 0;
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

        private const int TxProposalWindow = 12;

        public static (DisplayInput[] DisplayInputs, DisplayOutput[] DisplayOutputs) GenerateCellbaseDisplayInfos(Client client, string txHash, Output[] outputs, UInt64 blockNumber, string prefix)
        {
            if (blockNumber < (UInt64)TxProposalWindow)
            {
                var dInputs = new DisplayInput[]
                {
                            new DisplayInput
                            {
                                FromCellbase = true,
                                Capacity = "",
                                AddressHash = "",
                                TargetBlockNumber = "0",
                                GeneratedTxHash = txHash,
                            }
                };
                var dOutputs = Array.Empty<DisplayOutput>();
                return (dInputs, dOutputs);
            }
            UInt64 targetBlockNumber = blockNumber + 1 - (UInt64)TxProposalWindow;
            var displayInputs = new DisplayInput[]
            {
                            new DisplayInput
                            {
                                FromCellbase = true,
                                Capacity = "",
                                AddressHash = "",
                                TargetBlockNumber = targetBlockNumber.ToString(),
                                GeneratedTxHash = txHash,
                            }
            };
            Header? targetBlockHeader = client.GetHeaderByNumber(targetBlockNumber);
            if (targetBlockHeader == null)
            {
                throw new Exception("Target header not found!");
            }
            BlockEconomicState? targetEconomicState = client.GetBlockEconomicState(targetBlockHeader.Hash);
            if (targetEconomicState == null)
            {
                throw new Exception("Target economic state not found!");
            }
            MinerReward minerReward = targetEconomicState.MinerReward;
            var displayOutputs = outputs.Select((output, i) =>
            {
                return new DisplayOutput
                {
                    Id = $"{txHash}:{i}",
                    Capacity = Hex.HexToUInt64(output.Capacity).ToString(),
                    AddressHash = Ckb.Address.Address.GenerateAddress(output.Lock, prefix),
                    TargetBlockNumber = targetBlockNumber.ToString(),

                    PrimaryReward = Hex.HexToUInt64(minerReward.Primary).ToString(),
                    SecondaryReward = Hex.HexToUInt64(minerReward.Secondary).ToString(),
                    CommitReward = Hex.HexToUInt64(minerReward.Committed).ToString(),
                    ProposalReward = Hex.HexToUInt64(minerReward.Proposal).ToString(),

                    // TODO: update Status & ConsumedTxHash
                    Status = "live",
                    ConsumedTxHash = "",
                };
            }).ToArray();
            return (displayInputs, displayOutputs);
        }

        public static (DisplayInput[] DisplayInputs, DisplayOutput[] DisplayOutputs) GenerateNotCellbaseDisplayInfos(Input[] inputs, Output[] outputs, Output[] previousOutputs, string prefix, string txHash)
        {
            var displayInputs = inputs.Select((input, idx) =>
            {
                Output previousOutput = previousOutputs[idx];
                return new DisplayInput
                {
                    Id = $"{input.PreviousOutput.TxHash}:{Hex.HexToInt32(input.PreviousOutput.Index)}",
                    FromCellbase = false,
                    Capacity = Hex.HexToUInt64(previousOutput.Capacity).ToString(),
                    AddressHash = Ckb.Address.Address.GenerateAddress(previousOutput.Lock, prefix),
                    GeneratedTxHash = input.PreviousOutput.TxHash,
                    CellIndex = Hex.HexToUInt32(input.PreviousOutput.Index).ToString(),
                    // TODO: Need support "normal", "nervos_dao_deposit", "nervos_dao_withdrawing", "udt"
                    CellType = "normal"
                };
            }).ToArray();

            var displayOutputs = outputs.Select((output, i) =>
            {
                return new DisplayOutput
                {
                    Id = $"{txHash}:{i}",
                    Capacity = Hex.HexToUInt64(output.Capacity).ToString(),
                    AddressHash = Ckb.Address.Address.GenerateAddress(output.Lock, prefix),
                    // TODO: upate this.
                    Status = "live",
                    ConsumedTxHash = "",
                    // TODO: same in DisplayInputs
                    CellType = "normal"
                };
            }).ToArray();

            return (displayInputs, displayOutputs);
        }
    }
}
