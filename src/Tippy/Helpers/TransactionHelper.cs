using System;
using System.Linq;
using Ckb.Rpc;
using Ckb.Types;
using Tippy.Util;
using Tippy.ApiData;

namespace Tippy.Helpers
{
    public static class TransactionHelper
    {
        public const string EmptyHash = "0x0000000000000000000000000000000000000000000000000000000000000000";
        private const int TxProposalWindow = 12;

        public static string CkbAmount(string capacity) => NumberHelper.CkbAmount(capacity);

        // For not cellbase
        public static Output[] GetPreviousOutputs(Client client, Input[] inputs)
        {
            return inputs.Select(input => GetPreviousOutput(client, input)).ToArray();
        }

        public static Output GetPreviousOutput(Client client, Input input)
        {
            TransactionWithStatus? txWithStatus = client.GetTransaction(input.PreviousOutput.TxHash);
            if (txWithStatus == null)
            {
                throw new Exception("");
            }
            return txWithStatus.Transaction.Outputs[Hex.HexToUInt32(input.PreviousOutput.Index)];
        }

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
