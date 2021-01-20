using System;
using System.Globalization;

namespace Ckb.Molecule.Base
{
    public class UInt32Serializer : BaseSerializer<uint>
    {
        public override byte[] Header => new byte[0];
        public override byte[] Body => UInt32ToLEBytes(Value);

        public UInt32Serializer(uint value) : base(value) { }

        public UInt32Serializer(string hex) : base(0)
        {
            Value = UInt32.Parse(hex.Remove(0, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
        }
    }

    public class UInt64Serializer : BaseSerializer<ulong>
    {
        public override byte[] Header => new byte[0];
        public override byte[] Body => UInt64ToLEBytes(Value);

        public UInt64Serializer(uint value) : base(value) { }

        public UInt64Serializer(string hex) : base(0)
        {
            Value = UInt64.Parse(hex.Remove(0, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
        }
    }
}
