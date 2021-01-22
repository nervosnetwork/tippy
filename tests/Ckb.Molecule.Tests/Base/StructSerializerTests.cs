using Ckb.Molecule.Base;
using Xunit;

namespace Ckb.Molecule.Tests.Base
{
    class OnlyAByte
    {
        public byte F1;

        public OnlyAByte(byte f1)
        {
            F1 = f1;
        }
    }

    class ByteAndUInt32
    {
        public byte F1;
        public uint F2;
    }

    public class StructSerializerTests
    {
        [Fact]
        public void TestOnlyAByte()
        {
            var obj = new OnlyAByte(0xab);
            var onlyAByteSerializer = new StructSerializer(new BaseSerializer[] { new ByteSerializer(obj.F1) });
            var expected = new byte[] { 0xab };
            Assert.Equal(expected, onlyAByteSerializer.Serialize());
        }

        [Fact]
        public void TestByteAndUInt32()
        {
            var obj = new ByteAndUInt32()
            {
                F1 = 0xab,
                F2 = 0x010203,
            };
            var byteAndUInt32Serializer = new StructSerializer(new BaseSerializer[] { new ByteSerializer(obj.F1), new UInt32Serializer(obj.F2) });
            byte[] expected = new byte[] { 0xab, 0x03, 0x02, 0x01, 0x00 };
            Assert.Equal(expected, byteAndUInt32Serializer.Serialize());
        }
    }
}
