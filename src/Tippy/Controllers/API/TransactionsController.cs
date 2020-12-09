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
        private const string EmptyHash = "0x0000000000000000000000000000000000000000000000000000000000000000";
        private const int TxProposalWindow = 12;

        private Client? Rpc()
        {
            Project? activeProject = CurrentRunningProject();
            if (activeProject != null)
            {
                return new Client($"http://localhost:{activeProject.NodeRpcPort}");
            }

            return null;
        }

        private bool IsMainnet()
        {
            return CurrentRunningProject()?.Chain == Project.ChainType.Mainnet;
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

        [HttpGet("{hash}")]
        public ActionResult Details(string hash)
        {
            Client? client = Rpc();
            if (client == null)
            {
                return NoContent();
            }

            TransactionWithStatus? transactionWithStatus = client.GetTransaction(hash);
            if (transactionWithStatus == null)
            {
                return NoContent();
            }

            Transaction tx = transactionWithStatus.Transaction;

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
                TransactionHash = hash,
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

                    if (isCellbase && blockNumber < (UInt64)TxProposalWindow)
                    {
                        detail.DisplayInputs = new DisplayInput[]
                        {
                            new DisplayInput
                            {
                                FromCellbase = true,
                                Capacity = "",
                                AddressHash = "",
                                TargetBlockNumber = "0",
                                GeneratedTxHash = hash,
                            }
                        };
                        detail.DisplayOutputs = Array.Empty<DisplayOutput>();
                    }
                    else if (isCellbase)
                    {
                        UInt64 targetBlockNumber = blockNumber + 1 - (UInt64)TxProposalWindow;
                        detail.DisplayInputs = new DisplayInput[]
                        {
                            new DisplayInput
                            {
                                FromCellbase = true,
                                Capacity = "",
                                AddressHash = "",
                                TargetBlockNumber = targetBlockNumber.ToString(),
                                GeneratedTxHash = hash,
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
                        detail.DisplayOutputs = tx.Outputs.Select(output =>
                        {
                            return new DisplayOutput
                            {
                                Capacity = Hex.HexToUInt64(output.Capacity).ToString(),
                                AddressHash = Ckb.Address.Address.GenerateAddress(output.Lock, prefix),
                                TargetBlockNumber = targetBlockNumber.ToString(),

                                PrimaryReward = Hex.HexToUInt64(targetEconomicState.MinerReward.Primary).ToString(),
                                SecondaryReward = Hex.HexToUInt64(targetEconomicState.MinerReward.Secondary).ToString(),
                                CommitReward = Hex.HexToUInt64(targetEconomicState.MinerReward.Committed).ToString(),
                                ProposalReward = Hex.HexToUInt64(targetEconomicState.MinerReward.Proposal).ToString(),

                                // TODO: update Status & ConsumedTxHash
                                Status = "live",
                                ConsumedTxHash = "",
                            };
                        }).ToArray();
                    }
                }
            }

            if (!isCellbase)
            {
                var previousOutputs = GetPreviousOutputs(client, tx.Inputs);

                UInt64 transactionFee = previousOutputs.Select(p => Hex.HexToUInt64(p.Capacity)).Aggregate((sum, cur) => sum + cur) -
                    tx.Outputs.Select(o => Hex.HexToUInt64(o.Capacity)).Aggregate((sum, cur) => sum + cur);

                detail.TransactionFee = transactionFee.ToString();
                detail.DisplayInputs = tx.Inputs.Take(10).Select((input, idx) =>
                {
                    Output previousOutput = previousOutputs[idx];
                    return new DisplayInput
                    {
                        FromCellbase = false,
                        Capacity = Hex.HexToUInt64(previousOutput.Capacity).ToString(),
                        AddressHash = Ckb.Address.Address.GenerateAddress(previousOutput.Lock, prefix),
                        GeneratedTxHash = input.PreviousOutput.TxHash,
                        CellIndex = Hex.HexToUInt32(input.PreviousOutput.Index).ToString(),
                        // TODO: Need support "normal", "nervos_dao_deposit", "nervos_dao_withdrawing", "udt"
                        CellType = "normal"
                    };
                }).ToArray();

                detail.DisplayOutputs = tx.Outputs.Take(10).Select(output =>
                {
                    return new DisplayOutput
                    {
                        Capacity = Hex.HexToUInt64(output.Capacity).ToString(),
                        AddressHash = Ckb.Address.Address.GenerateAddress(output.Lock, prefix),
                        // TODO: upate this.
                        Status = "live",
                        ConsumedTxHash = "",
                        // TODO: same in DisplayInputs
                        CellType = "normal"
                    };
                }).ToArray();
            }

            Result<TransactionDetailResult> result = new("ckb_transaction", detail);

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

        // For not cellbase
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
            var displayOutputs = outputs.Select(output =>
            {
                return new DisplayOutput
                {
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

        public static (DisplayInput[] DisplayInputs, DisplayOutput[] DisplayOutputs) GenerateNotCellbaseDisplayInfos(Input[] inputs, Output[] outputs, Output[] previousOutputs, string prefix)
        {
            var displayInputs = inputs.Select((input, idx) =>
            {
                Output previousOutput = previousOutputs[idx];
                return new DisplayInput
                {
                    FromCellbase = false,
                    Capacity = Hex.HexToUInt64(previousOutput.Capacity).ToString(),
                    AddressHash = Ckb.Address.Address.GenerateAddress(previousOutput.Lock, prefix),
                    GeneratedTxHash = input.PreviousOutput.TxHash,
                    CellIndex = Hex.HexToUInt32(input.PreviousOutput.Index).ToString(),
                    // TODO: Need support "normal", "nervos_dao_deposit", "nervos_dao_withdrawing", "udt"
                    CellType = "normal"
                };
            }).ToArray();

            var displayOutputs = outputs.Select(output =>
            {
                return new DisplayOutput
                {
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
