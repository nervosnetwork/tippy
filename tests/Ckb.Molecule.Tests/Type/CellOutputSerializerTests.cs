using Ckb.Molecule.Type;
using Ckb.Types;
using Xunit;

namespace Ckb.Molecule.Tests.Type
{
    public class CellOutputSerializerTests
    {
        // This case from: https://github.com/nervosnetwork/ckb-sdk-js/blob/v0.39.0/packages/ckb-sdk-utils/__tests__/serialization/transaction/fixtures.json#L128
        [Fact]
        public void TestSerialize()
        {
            var output = new Output
            {
                Capacity = "0x174876e800",
                Lock = new Script
                {
                    CodeHash = "0x68d5438ac952d2f584abf879527946a537e82c7f3c1cbf6d8ebf9767437d8e88",
                    HashType = "type",
                    Args = "0x59a27ef3ba84f061517d13f42cf44ed020610061",
                },
                Type = new Script
                {
                    CodeHash = "0xece45e0979030e2f8909f76258631c42333b1e906fd9701ec3600a464a90b8f6",
                    HashType = "data",
                    Args = "0x",
                }
            };

            var expected = "0x9600000010000000180000006100000000e87648170000004900000010000000300000003100000068d5438ac952d2f584abf879527946a537e82c7f3c1cbf6d8ebf9767437d8e88011400000059a27ef3ba84f061517d13f42cf44ed02061006135000000100000003000000031000000ece45e0979030e2f8909f76258631c42333b1e906fd9701ec3600a464a90b8f60000000000";
            Assert.Equal(CellOutputSerializer.HexStringToBytes(expected), new CellOutputSerializer(output).Serialize());
        }
    }
}
