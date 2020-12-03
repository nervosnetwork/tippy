using System;
using System.Linq;

namespace Ckb.Types
{
    public class CellbaseWitness
    {
        public static Script Parse(string witness)
        {
            byte[] witnessSerialization = Convert.HexStringToBytes(witness);

            byte[] s = witnessSerialization.Skip(4).Take(4).ToArray();

            UInt32 scriptOffset = Convert.LEBytesToUInt32(s);

            UInt32 messageOffset = Convert.LEBytesToUInt32(
                witnessSerialization.Skip(8).Take(4).ToArray()
            );

            byte[] scriptSerialization = witnessSerialization
                .Skip((int)scriptOffset)
                .Take((int)(messageOffset - scriptOffset))
                .ToArray();
            UInt32 codeHashOffset = Convert.LEBytesToUInt32(
                scriptSerialization.Skip(4).Take(4).ToArray()
                );
            UInt32 hashTypeOffset = Convert.LEBytesToUInt32(
                scriptSerialization.Skip(8).Take(4).ToArray()
                );
            UInt32 argsOffset = Convert.LEBytesToUInt32(
                scriptSerialization.Skip(12).Take(4).ToArray());
            byte[] codeHashSerialization = scriptSerialization
                .Skip((int)codeHashOffset)
                .Take((int)(hashTypeOffset - codeHashOffset))
                .ToArray();
            byte[] hashTypeSerialization = scriptSerialization
                .Skip((int)hashTypeOffset)
                .Take((int)(argsOffset - hashTypeOffset))
                .ToArray();
            byte[] argsSerialization = scriptSerialization
                .Skip((int)hashTypeOffset + 5)
                .ToArray();

            string codeHash = Convert.BytesToHexString(codeHashSerialization);
            string hashType = Convert.BytesToHexString(hashTypeSerialization) == "0x00" ? "data" : "type";
            string args = Convert.BytesToHexString(argsSerialization);

            return new Script()
            {
                CodeHash = codeHash,
                HashType = hashType,
                Args = args,
            };
        }
    }
}
