using System;
using System.Globalization;
using System.Linq;
using System.Numerics;
using Ckb.Types;

namespace Ckb.Molecule.Base
{
    public class UInt32Serializer : BaseSerializer<uint>
    {
        public override byte[] Header => Array.Empty<byte>();
        public override byte[] Body => UInt32ToLEBytes(Value);

        public UInt32Serializer(uint value) : base(value) { }

        public UInt32Serializer(string hex) : base(0)
        {
            Value = UInt32.Parse(hex.Remove(0, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
        }
    }

    public class UInt64Serializer : BaseSerializer<ulong>
    {
        public override byte[] Header => Array.Empty<byte>();
        public override byte[] Body => UInt64ToLEBytes(Value);

        public UInt64Serializer(uint value) : base(value) { }

        public UInt64Serializer(string hex) : base(0)
        {
            Value = UInt64.Parse(hex.Remove(0, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
        }
    }

    public class UInt128Serializer : BaseSerializer<BigInteger>
    {
        public override byte[] Header => Array.Empty<byte>();
        public override byte[] Body => UInt128ToLEBytes(Value);

        public UInt128Serializer(BigInteger value) : base(value) { }

        public UInt128Serializer(string hex) : base(Types.Convert.HexToBigInteger(hex)) { }

        public static byte[] UInt128ToLEBytes(BigInteger num)
        {
            byte[] bytes = num.ToByteArray(true, false);
            byte[] uint128 = new byte[16];
            foreach (var (v, i) in bytes.Select((value, i) => (value, i)))
            {
                uint128[i] = v;
            }
            return uint128;
        }
    }
}
