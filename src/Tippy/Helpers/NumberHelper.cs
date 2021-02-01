using System;
using Tippy.Util;

namespace Tippy.Helpers
{
    public static class NumberHelper
    {
        public static string CkbAmount(string capacity)
        { 
            return (decimal.Parse(capacity) / 100_000_000).ToString();
        }

        public static string HexToNumber(string hex)
        {
            return Hex.HexToUInt64(hex).ToString();
        }
    }
}
