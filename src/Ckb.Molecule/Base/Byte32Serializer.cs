using System;

using HexString = System.String;

namespace Ckb.Molecule.Base
{
    public class Byte32Serializer : ArraySerializer<byte, ByteSerializer>
    {
        public Byte32Serializer(HexString str) : base(Array.Empty<byte>())
        {
            byte[] data = HexStringToBytes(str);
            if (data.Length != 32)
            {
                throw new Exception("Must be 32 bytes!");
            }
            Value = data;
        }
    }
}
