using System.Numerics;
using Xunit;

namespace Ckb.Types.Tests
{
    class CompactToTargetGroup
    {
        public uint Compact { get; set; }
        public BigInteger Ret { get; set; }
        public bool Overflow { get; set; }

        public CompactToTargetGroup(uint compact, BigInteger ret, bool overflow)
        {
            Compact = compact;
            Ret = ret;
            Overflow = overflow;
        }
    }

    public class DifficultyTests
    {
        private static readonly CompactToTargetGroup[] fixtures =
        {
            new CompactToTargetGroup(0, new BigInteger(0x0), false),
            new CompactToTargetGroup(0x123456, new BigInteger(0x0), false),
            new CompactToTargetGroup(0x1003456, new BigInteger(0x0), false),
            new CompactToTargetGroup(0x2000056, new BigInteger(0x0), false),
            new CompactToTargetGroup(0x3000000, new BigInteger(0x0), false),
            new CompactToTargetGroup(0x4000000, new BigInteger(0x0), false),
            new CompactToTargetGroup(0x923456, new BigInteger(0x0), false),
            new CompactToTargetGroup(0x1803456, new BigInteger(0x80), false),
            new CompactToTargetGroup(0x2800056, new BigInteger(0x8000), false),
            new CompactToTargetGroup(0x3800000, new BigInteger(0x800000), false),
            new CompactToTargetGroup(0x4800000, new BigInteger(0x80000000), false),
            new CompactToTargetGroup(0x1020000, new BigInteger(0x2), false),
            new CompactToTargetGroup(0x1fedcba, new BigInteger(0xfe), false),
            new CompactToTargetGroup(0x2123456, new BigInteger(0x1234), false),
            new CompactToTargetGroup(0x3123456, new BigInteger(0x123456), false),
            new CompactToTargetGroup(0x4123456, new BigInteger(0x12345600), false),
            new CompactToTargetGroup(0x4923456, new BigInteger(0x92345600), false),
            new CompactToTargetGroup(0x4923400, new BigInteger(0x92340000), false),
            new CompactToTargetGroup(0x20123456, Convert.HexToBigInteger("0x1234560000000000000000000000000000000000000000000000000000000000"), false),
            new CompactToTargetGroup(0xff123456, new BigInteger(0x0), true), // ignore ret
        };

        [Fact]
        public void TestCompactToTargetGroup()
        {
            foreach (var fixture in fixtures)
            {
                var (ret, overflow) = Difficulty.CompactToTarget(fixture.Compact);

                if (!fixture.Overflow)
                {
                    Assert.Equal(ret, fixture.Ret);
                }
                Assert.Equal(overflow, fixture.Overflow);
            }
        }
    }
}
