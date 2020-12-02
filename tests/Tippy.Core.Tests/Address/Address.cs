using Xunit;

using CoreAddress = Tippy.Core.Address.Address;

namespace Tippy.Core.Tests.Address
{
    public class Address
    {
        [Fact]
        public void TestGenerateAddress()
        {
            foreach (var prefix in new string[] { Addresses.TestnetPrefix, Addresses.MainnetPrefix })
            {
                foreach (var fixture in Addresses.GetAll())
                {
                    var result = CoreAddress.GenerateAddress(fixture.Script(), prefix);

                    if (prefix == Addresses.TestnetPrefix)
                    {
                        Assert.Equal(fixture.TestnetAddress, result);
                    }
                    else
                    {
                        Assert.Equal(fixture.MainnetAddress, result);
                    }
                }
            }
        }

        [Fact]
        public void TestnetParseAddress()
        {
            foreach (var fixture in Addresses.GetAll())
            {
                var prefix = "ckt";
                var result = CoreAddress.ParseAddress(fixture.TestnetAddress, prefix);
                Assert.Equal(fixture.CodeHash, result.CodeHash);
                Assert.Equal(fixture.HashType, result.HashType);
                Assert.Equal(fixture.Args, result.Args);
            }
        }

        [Fact]
        public void MainnetParseAddress()
        {
            foreach (var fixture in Addresses.GetAll())
            {
                var prefix = "ckb";
                var result = CoreAddress.ParseAddress(fixture.MainnetAddress, prefix);
                Assert.Equal(fixture.CodeHash, result.CodeHash);
                Assert.Equal(fixture.HashType, result.HashType);
                Assert.Equal(fixture.Args, result.Args);
            }
        }
    }
}
