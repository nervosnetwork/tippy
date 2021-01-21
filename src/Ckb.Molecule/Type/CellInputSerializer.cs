using Ckb.Molecule.Base;
using Ckb.Types;

namespace Ckb.Molecule.Type
{
    public class CellInputSerializer : StructSerializer<Input>
    {
        public CellInputSerializer(Input input)
            : base(input, new BaseSerializer[] { new UInt64Serializer(input.Since), new OutPointSerializer(input.PreviousOutput) }) { }
    }
}
