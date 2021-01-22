using Ckb.Molecule.Base;
using Ckb.Types;

namespace Ckb.Molecule.Type
{
    public class CellDepSerializer : StructSerializer
    {
        public CellDepSerializer(CellDep cellDep)
            : base(new BaseSerializer[] { new OutPointSerializer(cellDep.OutPoint), new ByteSerializer((byte)(cellDep.DepType == "code" ? 0x0 : 0x1)) }) { }
    }
}
