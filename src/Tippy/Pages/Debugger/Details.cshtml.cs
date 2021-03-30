using System;
using Ckb.Types.MockTransactionTypes;
using Ckb.Types;
using System.Linq;
using Ckb.Rpc;
using Ckb.Molecule.Type;
using Ckb.Cryptography;
using System.IO;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

using DebuggerProcessManager = Tippy.Ctrl.Process.Debugger.ProcessManager;
using TypesConvert = Ckb.Types.Convert;
using Microsoft.EntityFrameworkCore;
using Tippy.Core.Models;
using System.Threading.Tasks;

namespace Tippy.Pages.Debugger
{
    public class DetailsModel : PageModelBase
    {
        public DetailsModel(Tippy.Core.Data.TippyDbContext context) : base(context)
        {
        }

        [BindProperty]
        public string? FilePath { get; set; }
        public IActionResult OnPost(string? txHash, string? ioType, int? ioIndex, int? scriptType, int? txId = null)
        {
            if (FilePath == null)
            {
                throw new Exception("No file path provided!");
            }

            string url = txId == null ?
                $"/Debugger/Details?txHash={txHash}&ioType={ioType}&ioIndex={ioIndex}&scriptType={scriptType}&filePath={FilePath}"
                :
                $"/Debugger/Details?txId={txId}&ioType={ioType}&ioIndex={ioIndex}&scriptType={scriptType}&filePath={FilePath}";
            return Redirect(url);
        }

        public IActionResult OnPostClose()
        {
            DebuggerProcessManager.Stop();
            return Redirect("/Home");
        }

        public async Task<IActionResult> OnGet(string? txHash, string? ioType, int? ioIndex, int? scriptType, string? filePath, int? txId = null)
        {
            if (ioType != "input" && ioType != "output")
            {
                throw new Exception("ioType must be `input` or `output`!");
            }
            if (ioIndex == null)
            {
                throw new Exception("ioIndex cannot be null!");
            }
            if (scriptType == null)
            {
                throw new Exception("scriptType cannot be null!");
            }
            if (txId == null && (txHash == null || txHash == ""))
            {
                throw new Exception("txId and txHash cannot be all null!");
            }

            Client client = Rpc();

            Transaction transaction;
            if (txId != null)
            {
                FailedTransaction? failedTransaction = await DbContext.FailedTransactions.FirstOrDefaultAsync(t => t.Id == txId);
                if (failedTransaction == null)
                {
                    throw new Exception($"Failed transaction not found, check your txID: {txId}");
                }
                try
                {
                    transaction = Transaction.FromJson(failedTransaction.RawTransaction);
                }
                catch
                {
                    throw new Exception("Failed Transaction cannot parsed to Transaction!");
                }
            }
            else
            {
                TransactionWithStatus? txWithStatus = client.GetTransaction(txHash!);
                if (txWithStatus == null)
                {
                    throw new Exception($"Transaction not found: {txHash}");
                }
                transaction = txWithStatus.Transaction;
            }

            Output output = transaction.Outputs[(int)ioIndex];
            if (ioType == "input")
            {
                Input input = transaction.Inputs[(int)ioIndex];
                TransactionWithStatus originTx = client.GetTransaction(input.PreviousOutput.TxHash)!;
                output = originTx.Transaction.Outputs[TypesConvert.HexToUInt32(input.PreviousOutput.Index)];
            }
            if (scriptType == 1 && output.Type == null)
            {
                throw new Exception($"Type script not found in {{tx_hash: {txHash}, index: {ioIndex}, ioType: {ioType}}}");
            }

            Script script = scriptType == 0 ? output.Lock : output.Type;
            string scriptHash = ComputeScriptHash(script);
            MockTransaction mockTx = DumpTransaction(client, transaction);
            mockTx.Tx.Hash = null;

            string targetContractData = GetCellDepData(mockTx, script);

            string binaryFilePath = WriteToFile(scriptHash, targetContractData);

            string? binaryForDebugger = null;
            if (filePath != null)
            {
                binaryFilePath = filePath;
                binaryForDebugger = filePath;
            }

            string mockTxFilePath = WriteMockTx(scriptHash, mockTx.ToJson());

            string scriptGroupType = scriptType == 0 ? "lock" : "type";
            try
            {
                DebuggerProcessManager.Start(ActiveProject!, scriptGroupType, scriptHash, mockTxFilePath, binaryFilePath, ioType, (int)ioIndex, binaryForDebugger);
            }
            catch (System.InvalidOperationException e)
            {
                TempData["ErrorMessage"] = e.Message;
            }

            return Page();
        }

        private static string GetCellDepData(MockTransaction mockTx, Script script)
        {
            if (script.HashType == "data")
            {
                return GetCellDepDataByDataHash(mockTx, script.CodeHash);
            }
            return GetCellDepDataByTypeHash(mockTx, script.CodeHash);
        }

        private static string GetCellDepDataByDataHash(MockTransaction mockTx, string codeHash)
        {
            foreach (MockCellDep cellDep in mockTx.MockInfo.CellDeps)
            {
                string dataHash = ComputeDataHash(cellDep.Data);
                if (codeHash == dataHash)
                {
                    return cellDep.Data;
                }
            }
            throw new Exception("CellDep not found!");
        }

        private static string GetCellDepDataByTypeHash(MockTransaction mockTx, string codeHash)
        {
            foreach (MockCellDep cellDep in mockTx.MockInfo.CellDeps)
            {
                if (cellDep.Output.Type != null)
                {
                    string typeHash = ComputeScriptHash(cellDep.Output.Type);
                    if (codeHash == typeHash)
                    {
                        return cellDep.Data;
                    }
                }
            }
            throw new Exception("CellDep not found!");
        }

        private static string ComputeScriptHash(Script script)
        {
            return TypesConvert.BytesToHexString(Blake2bHasher.ComputeHash(new ScriptSerializer(script).Serialize()));
        }

        private static string ComputeDataHash(string data)
        {
            return TypesConvert.BytesToHexString(Blake2bHasher.ComputeHash(TypesConvert.HexStringToBytes(data)));
        }

        private static string WriteMockTx(string name, string jsonData)
        {
            string tempPath = Path.GetTempPath();
            string filePath = Path.Join(tempPath, "Tippy", "DebuggerBinaries", $"{name}.json");

            System.IO.File.WriteAllText(filePath, jsonData);

            return filePath;
        }

        private static string WriteToFile(string name, string data)
        {
            string tempPath = Path.GetTempPath();
            string filePathWithoutName = Path.Join(tempPath, "Tippy", "DebuggerBinaries");
            if (!Directory.Exists(filePathWithoutName))
            {
                Directory.CreateDirectory(filePathWithoutName);
            }
            string filePath = Path.Join(filePathWithoutName, name);

            using var stream = new FileStream(
                filePath,
                FileMode.Create,
                FileAccess.ReadWrite);

            var bytes = Ckb.Types.Convert.HexStringToBytes(data);
            stream.Write(bytes);

            stream.Close();

            return filePath;
        }

        private static MockTransaction DumpTransaction(Client client, Transaction tx)
        {
            MockInput[] mockInputs = tx.Inputs.Select((input) => GetMockInput(client, input)).ToArray();
            MockCellDep[] mockCellDeps = tx.CellDeps.SelectMany((cellDep) => GetMockCellDep(client, cellDep)).ToArray();
            Header[] mockHeaders = tx.HeaderDeps.Select((headerDep) => GetMockHeader(client, headerDep)).ToArray();

            MockTransaction mockTx = new MockTransaction
            {
                MockInfo = new MockInfo
                {
                    Inputs = mockInputs,
                    CellDeps = mockCellDeps,
                    HeaderDeps = mockHeaders,
                },
                Tx = tx
            };
            return mockTx;
        }

        private static MockInput GetMockInput(Client client, Input input)
        {
            TransactionWithStatus? txWithStatus = client.GetTransaction(input.PreviousOutput.TxHash);
            if (txWithStatus == null)
            {
                throw new Exception("Cannot find input cell");
            }
            int index = (int)TypesConvert.HexToUInt32(input.PreviousOutput.Index);

            MockInput mockInput = new MockInput
            {
                Input = input,
                Output = txWithStatus.Transaction.Outputs[index],
                Data = txWithStatus.Transaction.OutputsData[index],
            };

            return mockInput;
        }

        private static MockCellDep[] GetMockCellDep(Client client, CellDep cellDep)
        {
            TransactionWithStatus? txWithStatus = client.GetTransaction(cellDep.OutPoint.TxHash);
            if (txWithStatus == null)
            {
                throw new Exception($"Cannot find cell dep: {{ tx_hash: {cellDep.OutPoint.TxHash}, index: {cellDep.OutPoint.Index} }}!");
            }
            int index = (int)TypesConvert.HexToUInt32(cellDep.OutPoint.Index);

            string data = txWithStatus.Transaction.OutputsData[index];
            Output output = txWithStatus.Transaction.Outputs[index];

            List<MockCellDep> mockCellDeps = new();
            mockCellDeps.Add(new MockCellDep()
            {
                CellDep = cellDep,
                Output = output,
                Data = data,
            });

            if (cellDep.DepType == "dep_group")
            {
                CellDep[] cellDeps = UnpackDepGroup(data);
                foreach (CellDep dep in cellDeps)
                {
                    mockCellDeps.AddRange(GetMockCellDep(client, dep));
                }
            }

            return mockCellDeps.ToArray();
        }

        private static CellDep[] UnpackDepGroup(string data)
        {
            var outPoints = DeserializeOutPointVec(data);
            return outPoints.Select((outPoint) =>
            {
                return new CellDep()
                {
                    OutPoint = outPoint,
                    DepType = "code",
                };
            }).ToArray();
        }

        private static Header GetMockHeader(Client client, string headerDep)
        {
            Header? header = client.GetHeader(headerDep);
            if (header == null)
            {
                throw new Exception($"Cannot find header: {headerDep}");
            }
            return header;
        }

        private static OutPoint[] DeserializeOutPointVec(string data)
        {
            byte[] bytes = TypesConvert.HexStringToBytes(data);
            int size = (int)BitConverter.ToUInt32(bytes.Take(4).ToArray());
            int totalSize = bytes.Skip(4).ToArray().Length;
            int singleSize = totalSize / size;

            List<OutPoint> outPoints = new();

            for (int i = 0; i < size; i++)
            {
                int startAt = i * singleSize + 4;
                byte[] outPointBytes = bytes.Skip(startAt).Take(singleSize).ToArray();
                OutPoint outPoint = DeserializeOutPoint(outPointBytes);
                outPoints.Add(outPoint);
            }

            return outPoints.ToArray();
        }

        private static OutPoint DeserializeOutPoint(byte[] bytes)
        {
            byte[] txHashBytes = bytes.Take(32).ToArray();
            int index = (int)BitConverter.ToUInt32(bytes.Skip(32).Take(4).ToArray());

            OutPoint outPoint = new()
            {
                TxHash = TypesConvert.BytesToHexString(txHashBytes),
                Index = TypesConvert.Int32ToHex(index),
            };

            return outPoint;
        }
    }
}
