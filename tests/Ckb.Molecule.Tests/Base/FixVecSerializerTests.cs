using Ckb.Molecule.Base;
using Xunit;

namespace Ckb.Molecule.Tests.Base
{
    using ByteVecSerializer = FixVecSerializer<byte, ByteSerializer>;
    using UInt32VecSerializer = FixVecSerializer<uint, UInt32Serializer>;

    public class FixVecSerializerTests
    {
        [Fact]
        public void TestSerialize()
        {
            var value = new byte[] { };
            var expected = new byte[] { 0, 0, 0, 0 };
            var emptySerializer = new ByteVecSerializer(value);
            Assert.Equal(emptySerializer.Serialize(), expected);
        }

        [Fact]
        public void TestUInt32Vector()
        {
            var value = new uint[] { };
            var expected = new byte[] { 0, 0, 0, 0 };
            var emptySerializer = new UInt32VecSerializer(value);
            Assert.Equal(emptySerializer.Serialize(), expected);
        }

        [Fact]
        public void TestUInt32OneItemVector()
        {
            var value = new uint[] { 0x123 };
            var expected = new byte[] { 1, 0, 0, 0, 0x23, 1, 0, 0 };
            var oneItemSerializer = new UInt32VecSerializer(value);
            Assert.Equal(oneItemSerializer.Serialize(), expected);
        }

        [Fact]
        public void TestUInt32SixItemsSerilizer()
        {
            var value = new uint[] { 0x123, 0x456, 0x7890, 0xa, 0xbc, 0xdef };
            var expected = new byte[]
            {
                0x06, 0x00, 0x00, 0x00,
                0x23, 0x01, 0x00, 0x00,
                0x56, 0x04, 0x00, 0x00,
                0x90, 0x78, 0x00, 0x00,
                0x0a, 0x00, 0x00, 0x00,
                0xbc, 0x00, 0x00, 0x00,
                0xef, 0x0d, 0x00, 0x00
            };
            var sixItemsSerializer = new UInt32VecSerializer(value);
            Assert.Equal(sixItemsSerializer.Serialize(), expected);
        }
    }
}
