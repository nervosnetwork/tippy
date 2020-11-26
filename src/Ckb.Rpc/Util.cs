using System;
using System.Globalization;

namespace Ckb.Rpc
{
    public class Util
    {
        public static Int64 HexToInt64(string hex) =>
            Int64.Parse(hex.Remove(0, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);

        public static UInt64 HexToUInt64(string hex) =>
            UInt64.Parse(hex.Remove(0, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);

        public static string UInt64ToHex(UInt64 num) =>
            $"0x{num:x}";

    }
}
