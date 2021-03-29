using System;
using Tippy.Util;

namespace Tippy.Helpers
{
    public static class NumberHelper
    {
        public static string CkbAmount(string capacity) => Amount(capacity);

        public static string Amount(string amount, int decimalDigits = 8)
        {
            var result = String.Format(
                $"{{0:N{decimalDigits}}}",
                decimal.Parse(amount) / (decimal)Math.Pow(10, decimalDigits));
            while (result.EndsWith("0"))
            {
                result = result[0..^1];
            }
            if (result.EndsWith("."))
            {
                result = result[0..^1];
            }
            return result;
        }

        public static string HexToNumber(string hex)
        {
            return Hex.HexToUInt64(hex).ToString();
        }
    }
}
