using System;
using Ckb.Molecule.Base;

using HexString = System.String;

namespace Ckb.Molecule.Type
{
    public class ProposalShortIdSerializer : ArraySerializer<byte, ByteSerializer>
    {
        public ProposalShortIdSerializer(HexString str) : base(Array.Empty<byte>())
        {
            byte[] data = HexStringToBytes(str);
            if (data.Length != 10)
            {
                throw new Exception("Must be 10 bytes!");
            }
            Value = data;
        }

        public ProposalShortIdSerializer(byte[] bytes) : base(bytes) { }
    }
}
