using Xunit;

namespace Ckb.Types.Tests
{
    public class CellbaseWitnessTests
    {
        [Fact]
        public void TestParse()
        {
            string witness = "0x590000000c00000055000000490000001000000030000000310000009bd7e06f3ecf4be0f2fcd2188b23f1b9fcc88e5d4b65a8637b17723bbda3cce8011400000036c329ed630d6ce750712a477543672adab57f4c00000000";
            Script expectedScript = new()
            {
                CodeHash = "0x9bd7e06f3ecf4be0f2fcd2188b23f1b9fcc88e5d4b65a8637b17723bbda3cce8",
                HashType = "type",
                Args = "0x36c329ed630d6ce750712a477543672adab57f4c"
            };
            var script = CellbaseWitness.Parse(witness);
            Assert.Equal(expectedScript.CodeHash, script.CodeHash);
            Assert.Equal(expectedScript.HashType, script.HashType);
            Assert.Equal(expectedScript.Args, script.Args);
        }
    }
}
