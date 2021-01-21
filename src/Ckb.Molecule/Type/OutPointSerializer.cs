using Ckb.Molecule.Base;
using Ckb.Types;

namespace Ckb.Molecule
{
    public class OutPointSerializer : StructSerializer<OutPoint>
    {
        public OutPointSerializer(OutPoint outPoint)
            : base(outPoint, new BaseSerializer[] {
                new Byte32Serializer(outPoint.TxHash), new UInt32Serializer(outPoint.Index)
            })
        { }
    }
}
