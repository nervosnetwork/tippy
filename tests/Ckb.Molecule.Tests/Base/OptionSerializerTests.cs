using Ckb.Molecule.Base;
using Xunit;

namespace Ckb.Molecule.Tests.Base
{
    class TableType
    {
        public byte F1;
        public uint F2;

        public TableType(byte f1, uint f2)
        {
            F1 = f1;
            F2 = f2;
        }

        public TableSerializer<TableType> Serializer => new TableSerializer<TableType>(this, new BaseSerializer[] { new ByteSerializer(F1), new UInt32Serializer(F2) });
    }

    public class OptionSerializerTests
    {
        [Fact]
        public void TestNonEmptyObject()
        {
            var obj = new TableType(0x01, 2);
            var serializer = new TableSerializer<TableType>(obj, new BaseSerializer[] { new ByteSerializer(obj.F1), new UInt32Serializer(obj.F2) });
            var optionSerializer = new OptionSerializer<TableType, TableSerializer<TableType>>(obj, serializer);
            var expected = new byte[]
            {
                17, 0, 0, 0, 12, 0, 0, 0, 13, 0, 0, 0, 1, 2, 0, 0, 0
            };
            Assert.Equal(expected, optionSerializer.Serialize());
        }

        [Fact]
        public void TestEmptyObject()
        {
            TableType obj = null;
            var serializer = new TableSerializer<TableType>(obj, new BaseSerializer[] { new ByteSerializer(1), new UInt32Serializer(2) });
            var optionSerializer = new OptionSerializer<TableType, TableSerializer<TableType>>(obj, serializer);
            var expected = System.Array.Empty<byte>();
            Assert.Equal(expected, optionSerializer.Serialize());
        }
    }
}
