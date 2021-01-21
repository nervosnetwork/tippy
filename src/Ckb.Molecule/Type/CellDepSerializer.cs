using Ckb.Molecule.Base;
using Ckb.Types;

namespace Ckb.Molecule.Type
{
    public class CellDepSerializer : StructSerializer<CellDep>
    {
        public CellDepSerializer(CellDep cellDep)
            : base(cellDep, new BaseSerializer[] { new OutPointSerializer(cellDep.OutPoint), new ByteSerializer(cellDep.DepType == "code" ? 0x0 : 0x1) }) { }
    }
}
