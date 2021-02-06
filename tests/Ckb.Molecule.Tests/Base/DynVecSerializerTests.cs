using Ckb.Molecule.Base;
using Xunit;

namespace Ckb.Molecule.Tests.Base
{
    using BytesVecSerializer = DynVecSerializer<byte[], BytesSerializer>;

    public class DynVecSerializerTests
    {
        [Fact]
        public void TestEmptyBytesVector()
        {
            var emptySerializer = new BytesVecSerializer(System.Array.Empty<byte[]>());
            var expected = new byte[] { 4, 0, 0, 0 };

            Assert.Equal(emptySerializer.Serialize(), expected);
        }

        [Fact]
        public void TestOneItemBytesVector()
        {
            var value = new byte[][] { new byte[] { 0x12, 0x34 } };
            var expected = new byte[]
            {
                0x0e, 0x00, 0x00, 0x00,
                0x08, 0x00, 0x00, 0x00,
                0x02, 0x00, 0x00, 0x00, 0x12, 0x34
            };
            var oneItemSerializer = new BytesVecSerializer(value);
            Assert.Equal(expected, oneItemSerializer.Serialize());
        }

        [Fact]
        public void TestMultiItemsBytesVector()
        {
            var value = new byte[][]
            {
                new byte[] { 0x12, 0x34 },
                System.Array.Empty<byte>(),
                new byte[] { 0x05, 0x67 },
                new byte[] { 0x89 },
                new byte[] { 0xab, 0xcd, 0xef },
            };

            var expected = new byte[]
            {
                0x34, 0, 0, 0,
                0x18, 0, 0, 0, 0x1e, 0, 0, 0, 0x22, 0, 0, 0, 0x28, 0, 0, 0, 0x2d, 0, 0, 0,
                0x02, 0, 0, 0, 0x12, 0x34,
                0, 0, 0, 0,
                0x02, 0, 0, 0, 0x05, 0x67,
                0x01, 0, 0, 0, 0x89,
                0x03, 0, 0, 0, 0xab, 0xcd, 0xef
            };
            var multiItemsSerializer = new BytesVecSerializer(value);

            Assert.Equal(multiItemsSerializer.Serialize(), expected);
        }
    }
}
