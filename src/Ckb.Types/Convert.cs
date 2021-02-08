using System;
using System.Globalization;
using System.Linq;
using System.Numerics;

namespace Ckb.Types
{
    public static class Convert
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

        public static string Int32ToHex(int num) => $"0x{num:x}";

        public static byte[] HexStringToBytes(string hex)
        {
            //return System.Convert.FromHexString(hex.Remove(0, 2));
            string hexWithoutPrefix = hex.StartsWith("0x") ? hex.Remove(0, 2) : hex;
            int NumberChars = hexWithoutPrefix.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
            {
                bytes[i / 2] = System.Convert.ToByte(hexWithoutPrefix.Substring(i, 2), 16);
            }
            return bytes;
        }

        public static string BytesToHexString(byte[] bytes)
        {
            //return "0x" + System.Convert.ToHexString(bytes).ToLower();
            return "0x" + BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }

        public static UInt32 LEBytesToUInt32(byte[] bytes)
        {
            if (BitConverter.IsLittleEndian)
            {
                return BitConverter.ToUInt32(bytes, 0);
            }
            return BitConverter.ToUInt32(bytes.Reverse().ToArray(), 0);
        }

        public static UInt64 LEBytesToUInt64(byte[] bytes)
        {
            if (BitConverter.IsLittleEndian)
            {
                return BitConverter.ToUInt64(bytes, 0);
            }
            return BitConverter.ToUInt64(bytes.Reverse().ToArray(), 0);
        }

        public static BigInteger LEBytesToUInt128(byte[] bytes)
        {
            if (!BitConverter.IsLittleEndian)
            {
                bytes = bytes.Reverse().ToArray();
            }
            var high = BitConverter.ToUInt64(bytes, 0);
            var low = BitConverter.ToUInt64(bytes, 8);
            return (new BigInteger(low) << 64) + new BigInteger(high);
        }

        public static byte[] UInt128ToLEBytes(BigInteger num)
        {
            // ToByteArray return bytes in little-endian order
            byte[] bytes = num.ToByteArray();
            byte[] uint128 = new byte[16];
            foreach (var (v, i) in bytes.Select((value, i) => (value, i)))
            {
                uint128[i] = v;
            }
            return uint128;
        }
    }
}
