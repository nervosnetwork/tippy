using Ckb.Molecule.Base;
using Ckb.Types;

namespace Ckb.Molecule.Type
{
    public class RawHeaderSerializer : StructSerializer
    {
        public RawHeaderSerializer(Header header)
            : base(new BaseSerializer[]
            {
                new UInt32Serializer(header.Version),
                new UInt32Serializer(header.CompactTarget),
                new UInt64Serializer(header.Timestamp),
                new UInt64Serializer(header.Number),
                new UInt64Serializer(header.Epoch),
                new Byte32Serializer(header.ParentHash),
                new Byte32Serializer(header.TransactionsRoot),
                new Byte32Serializer(header.ProposalsHash),
                new Byte32Serializer(header.ExtraHash),
                new Byte32Serializer(header.Dao),
            })
        { }
    }
}
