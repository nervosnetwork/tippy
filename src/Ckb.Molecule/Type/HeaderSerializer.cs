using Ckb.Molecule.Base;
using Ckb.Types;

namespace Ckb.Molecule.Type
{
    public class HeaderSerializer : StructSerializer
    {
        public HeaderSerializer(Header header)
            : base(new BaseSerializer[]
            {
                new RawHeaderSerializer(header),
                new UInt128Serializer(header.Nonce),
            })
        { }
    }
}
