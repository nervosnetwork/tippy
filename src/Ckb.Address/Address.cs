using System;
using System.Collections.Generic;
using System.Linq;

namespace Ckb.Address
{
    public class Address
    {
        static readonly string SecpCodeHash = "0x9bd7e06f3ecf4be0f2fcd2188b23f1b9fcc88e5d4b65a8637b17723bbda3cce8";
        static readonly string SecpHashType = "type";
        static readonly int SecpShortId = 0;

        static readonly string MultisigCodeHash = "0x5c5069eb0857efc65e1bca0c07df34c31663b3622fd3876c876320fc9634e2a8";
        static readonly string MultisigHashType = "type";
        static readonly int MultisigShortId = 1;

        public static string GenerateAddress(Types.Script script, string prefix)
        {
            List<int> data = new();
            int? shortId = null;
            if (script.CodeHash == SecpCodeHash && script.HashType == SecpHashType)
            {
                shortId = SecpShortId;
            } else if (script.CodeHash == MultisigCodeHash && script.HashType == MultisigHashType)
            {
                shortId = MultisigShortId;
            }
            if (shortId != null)
            {
                data.Add(1);
                data.Add((int)shortId);
                foreach (byte c in Types.Convert.HexStringToBytes(script.Args))
                {
                    data.Add(Convert.ToInt32(c));
                }
            }
            else
            {
                data.Add(script.HashType == "type" ? 4 : 2);
                foreach (byte c in Types.Convert.HexStringToBytes(script.CodeHash))
                {
                    data.Add(Convert.ToInt32(c));
                }
                foreach (byte c in Types.Convert.HexStringToBytes(script.Args))
                {
                    data.Add(Convert.ToInt32(c));
                }
            }
            string addr = ConvertAddress.Encode(prefix, data.ToArray());

            return addr;
        }

        public static Types.Script ParseAddress(string address, string prefix)
        {
            (string hrp, int[] data) = ConvertAddress.Decode(address);

            if (hrp != prefix)
            {
                throw new Exception($"Invalid prefix! Expected: {prefix}, actual: {hrp}");
            }
            if (data[0] == 1)
            {
                if (data.Length < 2)
                {
                    throw new Exception("Invalid payload length!");
                }
                string codeHash;
                string hashType;
                if (data[1] == SecpShortId)
                {
                    codeHash = SecpCodeHash;
                    hashType = SecpHashType;
                } else if (data[1] == MultisigShortId)
                {
                    codeHash = MultisigCodeHash;
                    hashType = MultisigHashType;
                } else
                {
                    throw new Exception("Short address format error!");
                }

                return new Types.Script()
                {
                    CodeHash = codeHash,
                    HashType = hashType,
                    Args = IntsToHex(data.Skip(2).ToArray())
                };
            }
            else if (data[0] == 2 || data[0] == 4)
            {
                if (data.Length < 33)
                {
                    throw new Exception("Invalid payload length!");
                }

                return new Types.Script()
                {
                    CodeHash = IntsToHex(data.Skip(1).Take(32).ToArray()),
                    HashType = data[0] == 2 ? "data" : "type",
                    Args = IntsToHex(data.Skip(33).ToArray())
                };
            }
            throw new Exception($"Invalid payload format type: {data[0]}");
        }

        private static string IntsToHex(int[] values)
        {
            return Types.Convert.BytesToHexString(values.Select(v => (byte)v).ToArray());
        }
    }
}
