using System;
using System.Globalization;
using System.Linq;
using System.Numerics;

namespace Tippy.Util
{
    public class Hex
    {
        public static Int32 HexToInt32(string hex) =>
            Int32.Parse(hex.Remove(0, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);

        public static UInt32 HexToUInt32(string hex) =>
            UInt32.Parse(hex.Remove(0, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);

        public static Int64 HexToInt64(string hex) =>
            Int64.Parse(hex.Remove(0, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);

        public static UInt64 HexToUInt64(string hex) =>
            UInt64.Parse(hex.Remove(0, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);

        public static BigInteger HexToBigInteger(string hex)
        {
            string value = "0" + hex.Remove(0, 2);
            return BigInteger.Parse(value, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
        }

        public static string UInt64ToHex(UInt64 num) => $"0x{num:x}";

        public static string Int32ToHex(int num) => $"0x{num:x}";

        public static byte[] HexStringToBytes(string hex)
        {
            return Convert.FromHexString(hex.Remove(0, 2));
        }

        public static string BytesToHexString(byte[] bytes)
        {
            return "0x" + Convert.ToHexString(bytes).ToLower();
        }
    }
}
