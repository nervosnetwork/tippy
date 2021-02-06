using Ckb.Molecule.Base;
using Ckb.Types;

namespace Ckb.Molecule.Type
{
    public class CellInputSerializer : StructSerializer
    {
        public CellInputSerializer(Input input)
            : base(new BaseSerializer[] { new UInt64Serializer(input.Since), new OutPointSerializer(input.PreviousOutput) }) { }
    }
}
