using Ckb.Molecule.Base;
using Ckb.Types;

namespace Ckb.Molecule.Type
{
    public class ScriptSerializer : TableSerializer<Script>
    {
        public ScriptSerializer(Script script)
            : base(script, new BaseSerializer[] {
                new Byte32Serializer(script.CodeHash),
                new ByteSerializer(script.HashType == "data" ? 0x0 : 0x1),
                new BytesSerializer(script.Args),
            })
        { }
    }
}
