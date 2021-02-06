using Ckb.Molecule.Type;
using Ckb.Types;
using Xunit;

namespace Ckb.Molecule.Tests.Type
{
    public class ScriptSerializerTests
    {
        [Fact]
        public void TestSerialize()
        {
            var script = new Script
            {
                CodeHash = "0x68d5438ac952d2f584abf879527946a537e82c7f3c1cbf6d8ebf9767437d8e88",
                HashType = "type",
                Args = "0x3954acece65096bfa81258983ddb83915fc56bd8",
            };

            var expected = "4900000010000000300000003100000068d5438ac952d2f584abf879527946a537e82c7f3c1cbf6d8ebf9767437d8e8801140000003954acece65096bfa81258983ddb83915fc56bd8";
            var serializer = new ScriptSerializer(script);

            Assert.Equal(ScriptSerializer.HexStringToBytes(expected), serializer.Serialize());
        }
    }
}
