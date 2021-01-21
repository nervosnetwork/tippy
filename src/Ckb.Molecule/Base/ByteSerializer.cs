using System;

namespace Ckb.Molecule.Base
{
    public class ByteSerializer : BaseSerializer<byte>
    {
        public override byte[] Header => Array.Empty<byte>();

        public override byte[] Body => new byte[] { Value };

        public ByteSerializer(byte value) : base(value) { }

        public ByteSerializer(string value) : base(Convert.ToByte(value)) { }
    }
}
