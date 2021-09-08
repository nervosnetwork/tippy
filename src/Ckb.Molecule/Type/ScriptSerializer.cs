using Ckb.Molecule.Base;
using Ckb.Types;
using System;

namespace Ckb.Molecule.Type
{
    public class ScriptSerializer : TableSerializer
    {
        public ScriptSerializer(Script script)
            : base(new BaseSerializer[] {
                new Byte32Serializer(script.CodeHash),
                new ByteSerializer((byte) serializeHashType(script.HashType)),
                new BytesSerializer(script.Args),
            })
        { }

        private static byte serializeHashType(string hashType) {
            if (hashType == "data") {
                return 0x0;
            } else if (hashType == "type") {
                return 0x1;
            } else if (hashType == "data1") {
                return 0x2;
            }
            throw new Exception($"Invalid script hash_type: {hashType}");
        }
    }
}
