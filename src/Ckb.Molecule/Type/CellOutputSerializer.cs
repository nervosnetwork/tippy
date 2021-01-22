using Ckb.Molecule.Base;
using Ckb.Types;

namespace Ckb.Molecule.Type
{
    public class CellOutputSerializer : TableSerializer
    {
        public CellOutputSerializer(Output output)
            : base(new BaseSerializer[] {
                new UInt64Serializer(output.Capacity),
                new ScriptSerializer(output.Lock),
                new OptionSerializer<Script, ScriptSerializer>(output.Type, output.Type == null ? null : new ScriptSerializer(output.Type)),
            })
        { }
    }
}
