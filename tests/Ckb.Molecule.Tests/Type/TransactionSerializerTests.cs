using System;
using Ckb.Cryptography;
using Ckb.Molecule.Type;
using Ckb.Types;
using Xunit;

namespace Ckb.Molecule.Tests.Type
{
    public class TransactionSerializerTests
    {
        [Fact]
        public void TestSerializer()
        {
            var tx = new Transaction
            {
                Version = "0x0",
                CellDeps = new CellDep[]
                {
                    new CellDep
                    {
                        OutPoint = new OutPoint
                        {
                            TxHash = "0xbffab7ee0a050e2cb882de066d3dbf3afdd8932d6a26eda44f06e4b23f0f4b5a",
                            Index = "0x0",
                        },
                        DepType = "code",
                    }
                },
                HeaderDeps = Array.Empty<string>(),
                Inputs = Array.Empty<Input>(),
                Outputs = new Output[]
                {
                    new Output
                    {
                        Capacity = "0x" + 100000000000.ToString("x"),
                        Lock = new Script
                        {
                            CodeHash = "0x9e3b3557f11b2b3532ce352bfe8017e9fd11d154c4c7f9b7aaaa1e621b539a08",
                            HashType = "data",
                            Args = "0xe2193df51d78411601796b35b17b4f8f2cd85bd0",
                        }
                    },
                    new Output
                    {
                        Capacity = "0x" + 4900000000000.ToString("x"),
                        Lock = new Script
                        {
                            CodeHash = "0x9e3b3557f11b2b3532ce352bfe8017e9fd11d154c4c7f9b7aaaa1e621b539a08",
                            HashType = "data",
                            Args = "0x36c329ed630d6ce750712a477543672adab57f4c",
                        }
                    }
                },
                OutputsData = new string[] { "0x", "0x" },
                Witnesses = new string[] { "0x" },
            };

            var expected = "0x4c02905db773301f73bbc6cd5a400c928caf410bbb13136f6f48bec0a79c22e4";
            Assert.Equal(TransactionSerializer.HexStringToBytes(expected), Blake2bHasher.ComputeHash(new RawTransactionSerializer(tx).Serialize()));
        }

        [Fact]
        public void TestWithInputs()
        {
            var tx = new Transaction
            {
                Version = "0x0",
                CellDeps = new CellDep[]
                {
                    new CellDep
                    {
                        OutPoint = new OutPoint
                        {
                            TxHash = "0x29f94532fb6c7a17f13bcde5adb6e2921776ee6f357adf645e5393bd13442141",
                            Index = "0x0",
                        },
                        DepType = "code"
                    }
                },
                HeaderDeps = new string[]
                {
                    "0x8033e126475d197f2366bbc2f30b907d15af85c9d9533253c6f0787dcbbb509e"
                },
                Inputs = new Input[]
                {
                    new Input
                    {
                        PreviousOutput = new OutPoint
                        {
                            TxHash = "0x5ba156200c6310bf140fbbd3bfe7e8f03d4d5f82b612c1a8ec2501826eaabc17",
                            Index = "0x0"
                        },
                        Since = "0x0",
                    }
                },
                Outputs = new Output[]
                {
                    new Output
                    {
                        Capacity = "0x" + 100000000000.ToString("x"),
                        Lock = new Script
                        {
                            CodeHash = "0x28e83a1277d48add8e72fadaa9248559e1b632bab2bd60b27955ebc4c03800a5",
                            HashType = "data",
                            Args = "0x",
                        }
                    }
                },
                OutputsData = new string[] { "0x" },
            };

            var expected = "0x6f6b16079884d8127490aac5e0f87274e81f15ea1fd6c9672a5b0326bd8ce76d";
            Assert.Equal(RawTransactionSerializer.HexStringToBytes(expected), Blake2bHasher.ComputeHash(new RawTransactionSerializer(tx).Serialize()));
        }

        // This case from: https://github.com/nervosnetwork/ckb-sdk-js/blob/v0.39.0/packages/ckb-sdk-utils/__tests__/serialization/transaction/fixtures.json#L275
        [Fact]
        public void TestTransaction()
        {
            var tx = new Transaction
            {
                Version = "0x0",
                CellDeps = new CellDep[]
                {
                    new CellDep
                    {
                        OutPoint = new OutPoint
                        {
                            TxHash = "0xc12386705b5cbb312b693874f3edf45c43a274482e27b8df0fd80c8d3f5feb8b",
                            Index = "0x0"
                        },
                        DepType = "dep_group",
                    },
                    new CellDep
                    {
                        OutPoint = new OutPoint
                        {
                            TxHash = "0x0fb4945d52baf91e0dee2a686cdd9d84cad95b566a1d7409b970ee0a0f364f60",
                            Index = "0x2",
                        },
                        DepType = "code",
                    }
                },
                HeaderDeps = Array.Empty<string>(),
                Inputs = new Input[]
                {
                    new Input
                    {
                        PreviousOutput = new OutPoint
                        {
                            TxHash = "0x31f695263423a4b05045dd25ce6692bb55d7bba2965d8be16b036e138e72cc65",
                            Index = "0x1",
                        },
                        Since = "0x0",
                    }
                },
                Outputs = new Output[]
                {
                    new Output
                    {
                        Capacity = "0x174876e800",
                        Lock = new Script
                        {
                            CodeHash = "0x68d5438ac952d2f584abf879527946a537e82c7f3c1cbf6d8ebf9767437d8e88",
                            HashType  = "type",
                            Args = "0x59a27ef3ba84f061517d13f42cf44ed020610061",
                        },
                        Type = new Script
                        {
                            CodeHash = "0xece45e0979030e2f8909f76258631c42333b1e906fd9701ec3600a464a90b8f6",
                            HashType = "data",
                            Args = "0x",
                        }
                    },
                    new Output
                    {
                        Capacity = "0x59e1416a5000",
                        Lock = new Script
                        {
                            CodeHash = "0x68d5438ac952d2f584abf879527946a537e82c7f3c1cbf6d8ebf9767437d8e88",
                            HashType = "type",
                            Args = "0x59a27ef3ba84f061517d13f42cf44ed020610061",
                        },
                        Type = null,
                    }
                },
                OutputsData = new string[] { "0x", "0x" },
                Witnesses = new string[] { "0x82df73581bcd08cb9aa270128d15e79996229ce8ea9e4f985b49fbf36762c5c37936caf3ea3784ee326f60b8992924fcf496f9503c907982525a3436f01ab32900" },
            };

            var expected = "0x120200000c000000c5010000b90100001c000000200000006e00000072000000a2000000a50100000000000002000000c12386705b5cbb312b693874f3edf45c43a274482e27b8df0fd80c8d3f5feb8b00000000010fb4945d52baf91e0dee2a686cdd9d84cad95b566a1d7409b970ee0a0f364f6002000000000000000001000000000000000000000031f695263423a4b05045dd25ce6692bb55d7bba2965d8be16b036e138e72cc6501000000030100000c000000a20000009600000010000000180000006100000000e87648170000004900000010000000300000003100000068d5438ac952d2f584abf879527946a537e82c7f3c1cbf6d8ebf9767437d8e88011400000059a27ef3ba84f061517d13f42cf44ed02061006135000000100000003000000031000000ece45e0979030e2f8909f76258631c42333b1e906fd9701ec3600a464a90b8f600000000006100000010000000180000006100000000506a41e15900004900000010000000300000003100000068d5438ac952d2f584abf879527946a537e82c7f3c1cbf6d8ebf9767437d8e88011400000059a27ef3ba84f061517d13f42cf44ed020610061140000000c0000001000000000000000000000004d000000080000004100000082df73581bcd08cb9aa270128d15e79996229ce8ea9e4f985b49fbf36762c5c37936caf3ea3784ee326f60b8992924fcf496f9503c907982525a3436f01ab32900";

            Assert.Equal(TransactionSerializer.HexStringToBytes(expected), new TransactionSerializer(tx).Serialize());
        }
    }
}
