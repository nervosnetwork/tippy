using System;
using Xunit;

namespace Ckb.Types.Tests
{
    public class OutputTests
    {
        [Fact]
        public void TestMinimalCellCapacity()
        {
            Output output = new Output
            {
                Capacity = "0x174876e800",
                Lock = new Script
                {
                    CodeHash = "0x9bd7e06f3ecf4be0f2fcd2188b23f1b9fcc88e5d4b65a8637b17723bbda3cce8",
                    HashType = "type",
                    Args = "0x36c329ed630d6ce750712a477543672adab57f4c",
                },
                Data = "0x",
            };
            ulong expected = (ulong)61 * (ulong)Math.Pow(10, 8);

            Assert.Equal(expected, output.MinimalCellCapacity());
        }

        [Fact]
        public void TestMinimalCellCapacityWithNullData()
        {
            Output output = new Output
            {
                Capacity = "0x174876e800",
                Lock = new Script
                {
                    CodeHash = "0x9bd7e06f3ecf4be0f2fcd2188b23f1b9fcc88e5d4b65a8637b17723bbda3cce8",
                    HashType = "type",
                    Args = "0x36c329ed630d6ce750712a477543672adab57f4c",
                },
            };

            Assert.Equal((ulong)0, output.MinimalCellCapacity());
        }

        [Fact]
        public void TestMinimalCellCapacityWithType()
        {
            Output output = new Output
            {
                Capacity = "0x174876e800",
                Lock = new Script
                {
                    CodeHash = "0x9bd7e06f3ecf4be0f2fcd2188b23f1b9fcc88e5d4b65a8637b17723bbda3cce8",
                    HashType = "type",
                    Args = "0x36c329ed630d6ce750712a477543672adab57f4c",
                },
                Type = new Script
                {
                    CodeHash = "0x9e3b3557f11b2b3532ce352bfe8017e9fd11d154c4c7f9b7aaaa1e621b539a08",
                    HashType = "type",
                    Args = "0x",
                },
                Data = "0x1234",
            };
            ulong expected = (ulong)(61 + 33 + 2) * (ulong)Math.Pow(10, 8);

            Assert.Equal(expected, output.MinimalCellCapacity());
        }
    }
}
