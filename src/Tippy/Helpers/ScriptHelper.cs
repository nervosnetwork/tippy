using System;
using Ckb.Cryptography;
using Ckb.Molecule.Type;
using Ckb.Types;

namespace Tippy.Helpers
{
    public static class ScriptHelper
    {
        public static string ComputeHash(Script script)
        {
            var serializer = new ScriptSerializer(script);
            var hash = Blake2bHasher.ComputeHash(serializer.Serialize());
            return Util.Hex.BytesToHexString(hash);
        }
    }
}
