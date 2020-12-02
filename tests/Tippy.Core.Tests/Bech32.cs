using System;
using System.Linq;
using Xunit;

using Ckb.Address;

namespace Ckb.Address.Tests.Address
{
    class ValidAddressFixture
    {
        public string Address { get; set; }
        public string Prefix { get; set; }
        public string Hex { get; set; }
    }

    class InvalidAddresFixture
    {
        public string Address { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class Bech32Tests
    {
        static string[] ValidCheckSum = new string[]
        {
            "A12UEL5L",
            "an83characterlonghumanreadablepartthatcontainsthenumber1andtheexcludedcharactersbio1tt5tgs",
            "abcdef1qpzry9x8gf2tvdw0s3jn54khce6mua7lmqqqxw",
            "11qqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqc8247j",
            "split1checkupstagehandshakeupstreamerranterredcaperred2y9e3w",
        };

        static string[] InvalidChecksum = new string[]
        {
            " 1nwldj5",
            "\x7F" + "1axkwrx",
            //"an84characterslonghumanreadablepartthatcontainsthenumber1andtheexcludedcharactersbio1569pvx",
            "pzry9x0s0muk",
            "1pzry9x0s0muk",
            "x1b4n0q5v",
            "li1dgmt3",
            "de1lg7wt\xFF",
        };

        static ValidAddressFixture[] ValidAddresses = new ValidAddressFixture[]
        {
            new ValidAddressFixture()
            {
                Address = "A12UEL5L",
                Prefix = "A",
                Hex = "0x"
            },
            new ValidAddressFixture()
            {
                Address = "an83characterlonghumanreadablepartthatcontainsthenumber1andtheexcludedcharactersbio1tt5tgs",
                Prefix = "an83characterlonghumanreadablepartthatcontainsthenumber1andtheexcludedcharactersbio",
                Hex = "0x"
            },
            new ValidAddressFixture()
            {
                Address = "abcdef1qpzry9x8gf2tvdw0s3jn54khce6mua7lmqqqxw",
                Prefix = "abcdef",
                Hex = "0x00443214c74254b635cf84653a56d7c675be77df"
            },
            new ValidAddressFixture()
            {
                Address = "11qqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqc8247j",
                Prefix = "1",
                Hex = "0x000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000"
            },
            new ValidAddressFixture()
            {
                Address = "split1checkupstagehandshakeupstreamerranterredcaperred2y9e3w",
                Prefix = "split",
                Hex = "0xc5f38b70305f519bf66d85fb6cf03058f3dde463ecd7918f2dc743918f2d"
            },
            new ValidAddressFixture()
            {
                Address = "11qqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqq978ear",
                Prefix = "1",
                Hex = "0x000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000"
            },
    };

        static InvalidAddresFixture[] InvalidAddresses = new InvalidAddresFixture[]
        {
            new InvalidAddresFixture()
            {
                Address = "A12Uel5l",
                ErrorMessage = "mix case"
            },
            new InvalidAddresFixture()
            {
                Address = "pzry9x0s0muk",
                ErrorMessage = "separator '1' at invalid position"
            },
            new InvalidAddresFixture()
            {
                Address = "6465316c67377774ffo",
                ErrorMessage = "invalid character data part"
            }
        };

        [Fact]
        public void TestValidChecksum()
        {
            foreach (var t in ValidCheckSum)
            {
                Bech32.Decode(t);
            }
        }

        [Fact]
        public void TestInvalidChecksum()
        {
            foreach (var (t, i) in InvalidChecksum.Select((value, i) => (value, i)))
            {
                Console.WriteLine($"i: {i}");
                Assert.Throws<Exception>(() => Bech32.Decode(t));
            }
        }

        [Fact]
        public void TestValidAddresses()
        {
            foreach (var info in ValidAddresses)
            {
                // Decode
                (string hrp, int[] data) = ConvertAddress.Decode(info.Address);
                var hex = Types.Convert.BytesToHexString(
                    data.Select(d => (byte)d).ToArray());
                Assert.Equal(info.Prefix, hrp);
                Assert.Equal(info.Hex, hex);

                // Encode
                string addr = ConvertAddress.Encode(hrp, data);
                Assert.Equal(info.Address, addr);
            }
        }

        [Fact]
        public void TestInvalidAddresses()
        {
            foreach(var info in InvalidAddresses)
            {
                var exception = Assert.Throws<Exception>(() => ConvertAddress.Decode(info.Address));
                Assert.Equal(info.ErrorMessage, exception.Message);
            }
        }
    }
}
