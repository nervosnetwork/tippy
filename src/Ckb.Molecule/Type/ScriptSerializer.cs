using Ckb.Molecule.Base;
using Ckb.Types;

namespace Ckb.Molecule.Type
{
    public class ScriptSerializer : TableSerializer
    {
        public ScriptSerializer(Script script)
            : base(new BaseSerializer[] {
                new Byte32Serializer(script.CodeHash),
                new ByteSerializer((byte)(script.HashType == "data" ? 0x0 : 0x1)),
                new BytesSerializer(script.Args),
            })
        { }
    }
}
