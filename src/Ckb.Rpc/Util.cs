using System;
using System.Globalization;

namespace Ckb.Rpc
{
    public class Util
    {
        public static Int64 HexToInt64(string hex)
        {
            return Int64.Parse(hex.Remove(0, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
        }
    }
}
