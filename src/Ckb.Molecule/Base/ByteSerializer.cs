using System;

namespace Ckb.Molecule.Base
{
    public class ByteSerializer : BaseSerializer<byte>
    {
        public override byte[] Header => new Byte[0];

        public override byte[] Body => new byte[1] { Value };

        public ByteSerializer(byte value) : base(value) { }

        public ByteSerializer(string value) : base(Convert.ToByte(value)) { }
    }
}
