using Ckb.Molecule.Base;
using Xunit;

namespace Ckb.Molecule.Tests.Base
{
    class MixedType
    {
        public byte[] F1;
        public byte F2;
        public uint F3;
        public byte[] F4;
        public byte[] F5;
    }

    public class TableSerializerTests
    {
        [Fact]
        public void TestSerialize()
        {
            var obj = new MixedType()
            {
                F1 = System.Array.Empty<byte>(),
                F2 = 0xab,
                F3 = 0x123,
                F4 = new byte[] { 0x45, 0x67, 0x89 },
                F5 = new byte[] { 0xab, 0xcd, 0xef },
            };
            var serialzier = new TableSerializer(new BaseSerializer[]
                {
                    new BytesSerializer(obj.F1),
                    new ByteSerializer(obj.F2),
                    new UInt32Serializer(obj.F3),
                    new ArraySerializer<byte, ByteSerializer>(obj.F4),
                    new BytesSerializer(obj.F5),
                }
            );
            var expected = new byte[]
            {
                0x2b, 0, 0, 0,
                0x18, 0, 0, 0, 0x1c, 0, 0, 0, 0x1d, 0, 0, 0, 0x21, 0, 0, 0, 0x24, 0, 0, 0,
                0, 0, 0, 0,
                0xab,
                0x23, 0x01, 0, 0,
                0x45, 0x67, 0x89,
                0x03, 0, 0, 0, 0xab, 0xcd, 0xef
            };
            Assert.Equal(expected, serialzier.Serialize());
        }
    }
}
