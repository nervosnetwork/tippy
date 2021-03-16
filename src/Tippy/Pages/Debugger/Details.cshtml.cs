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


namespace Tippy.Pages.Debugger
{
    public class DetailsModel : PageModelBase
    {
        public DetailsModel(Tippy.Core.Data.DbContext context) : base(context)
        {
        }

        // TODO: upload file
        [BindProperty]
        public string? FilePath { get; set; }
        public IActionResult OnPost(string? txHash, string? ioType, int? ioIndex, int? scriptType)
        {
            if (FilePath == null)
            {
                throw new Exception("No file path provided!");
            }

            string url = $"/Debugger/Details?txHash={txHash}&ioType={ioType}&ioIndex={ioIndex}&scriptType={scriptType}&filePath={FilePath}";
            return Redirect(url);
        }

        public void OnGet(string? txHash, string? ioType, int? ioIndex, int? scriptType, string? filePath)
        {
            if (txHash == null)
            {
                throw new Exception("txHash cannot be null!");
            }
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

            Client client = Rpc();

            TransactionWithStatus? txWithStatus = client.GetTransaction(txHash);
            if (txWithStatus == null)
            {
                throw new Exception($"Transaction not found: {txHash}");
            }

            Output output = txWithStatus.Transaction.Outputs[(int)ioIndex];
            if (ioType == "input")
            {
                Input input = txWithStatus.Transaction.Inputs[(int)ioIndex];
                TransactionWithStatus originTx = client.GetTransaction(input.PreviousOutput.TxHash)!;
                output = originTx.Transaction.Outputs[TypesConvert.HexToUInt32(input.PreviousOutput.Index)];
            }
            if (scriptType == 1 && output.Type == null)
            {
                throw new Exception($"Type script not found in {{tx_hash: {txHash}, index: {ioIndex}, ioType: {ioType}}}");
            }

            Script script = scriptType == 0 ? output.Lock : output.Type;
            string scriptHash = ComputeScriptHash(script);
            MockTransaction mockTx = DumpTransaction(client, txHash, txWithStatus.Transaction);
            mockTx.Tx.Hash = null;

            string targetContractData = GetCellDepData(mockTx, script);

            string binaryFilePath = WriteToFile(scriptHash, targetContractData);

            if (filePath != null)
            {
                string binaryFileData = ReadHexFromBinaryFile(filePath);
                mockTx = ReplaceMockTxData(mockTx, script, binaryFileData);
                binaryFilePath = filePath;
            }

            string mockTxFilePath = WriteMockTx(scriptHash, mockTx.ToJson());

            string scriptGroupType = scriptType == 0 ? "lock" : "type";
            try
            {
                DebuggerProcessManager.Start(scriptGroupType, scriptHash, mockTxFilePath, binaryFilePath, ioType, (int)ioIndex);
            }
            catch (System.InvalidOperationException e)
            {
                TempData["ErrorMessage"] = e.Message;
            }
        }

        // TODO: Output.Lock may no cell dep, throw a more friendly error.
        private static string GetCellDepData(MockTransaction mockTx, Script script)
        {
            if (script.HashType == "data") {
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

        private static string ReadHexFromBinaryFile(string filePath)
        {
            byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
            string hex = TypesConvert.BytesToHexString(fileBytes);
            return hex;
        }

        private static MockTransaction ReplaceMockTxData(MockTransaction mockTx, Script script, string newData)
        {
            // find cell dep
            int mockCellDepIndex = -1;
            MockCellDep[] mockCellDeps = mockTx.MockInfo.CellDeps;
            for (int i = 0; i < mockCellDeps.Length; i++)
            {
                MockCellDep mockCellDep = mockCellDeps[i];
                if (script.HashType == "data")
                {
                    string dataHash = ComputeDataHash(mockCellDep.Data);
                    if (script.CodeHash == dataHash)
                    {
                        mockCellDepIndex = i;
                        break;
                    }
                }
                else
                {
                    if (mockCellDep.Output.Type != null)
                    {
                        string typeHash = ComputeScriptHash(mockCellDep.Output.Type);
                        if (script.CodeHash == typeHash)
                        {
                            mockCellDepIndex = i;
                            break;
                        }
                    }
                }
            }
            if (mockCellDepIndex == -1)
            {
                throw new Exception("No cell dep match!");
            }

            mockTx.MockInfo.CellDeps[mockCellDepIndex].Data = newData;
            return mockTx;
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
            if (!Directory.Exists(filePathWithoutName)) {
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

        private static MockTransaction DumpTransaction(Client client, string txHash, Transaction tx)
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
            return outPoints.Select((outPoint) => {
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
