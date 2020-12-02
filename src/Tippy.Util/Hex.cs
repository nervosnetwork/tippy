using System;
using System.Globalization;
using System.Linq;
using System.Numerics;

namespace Tippy.Util
{
    public class Hex
    {
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

        public static byte[] HexToBytes(string i)
        {
            string hex = i.Remove(0, 2);
            return Enumerable.Range(0, hex.Length)
                 .Where(x => x % 2 == 0)
                 .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                 .ToArray();
        }

        public static string BytesToHex(byte[] bytes)
        {
            return "0x" + string.Concat(bytes.Select(b => b.ToString("x2")).ToArray());
        }
    }
}
