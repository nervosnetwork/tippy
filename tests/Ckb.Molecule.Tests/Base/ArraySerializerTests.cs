using Ckb.Molecule.Base;
using Xunit;

namespace Ckb.Molecule.Tests.Base
{
    public class ArraySerializerTests
    {
        [Fact]
        public void TestByte3()
        {
            Assert.Equal(new ArraySerializer<byte, ByteSerializer>(new byte[3] { 0x01, 0x02, 0x03 }).Serialize(), new byte[3] { 0x01, 0x02, 0x03 });
        }

        [Fact]
        public void TestTwoUInt32()
        {
            uint[] value = new uint[] { 0x01020304, 0xabcde };
            byte[] result = new byte[] { 0x04, 0x03, 0x02, 0x01, 0xde, 0xbc, 0x0a, 0x00 };

            Assert.Equal(new ArraySerializer<uint, UInt32Serializer>(value).Serialize(), result);
        }
    }
}
