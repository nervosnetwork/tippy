using System.Numerics;
using Xunit;

namespace Ckb.Types.Tests
{
    public class ConvertTests
    {
        [Fact]
        public void TestLEBytesToUInt128()
        {
            string hex = "0xe7584257010000000000000000000000";
            BigInteger expected = BigInteger.Parse("5758933223");

            byte[] bytes = Convert.HexStringToBytes(hex);
            var le = Convert.LEBytesToUInt128(bytes);
            Assert.Equal(expected, le);
        }
    }
}
