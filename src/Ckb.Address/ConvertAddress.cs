using System;
using System.Collections.Generic;

namespace Ckb.Address
{
    public class ConvertAddress
    {
        static int[] ConvertBits(int[] data, uint fromBits, uint toBits, bool pad)
        {
            int acc = 0;
            uint bits = 0;
            List<int> ret = new();
            int maxv = (1 << (int)toBits) - 1;
            foreach (int value in data)
            {
                if (value < 0 || (value >> (int)fromBits) != 0)
                {
                    throw new Exception("invalid data range");
                }
                acc = (acc << (int)fromBits) | value;
                bits += fromBits;

                while (bits >= toBits)
                {
                    bits -= toBits;
                    ret.Add((acc >> (int)bits) & maxv);
                }
            }
            if (pad)
            {
                if (bits > 0)
                {
                    ret.Add((acc << (int)(toBits - bits)) & maxv);
                }
            }
            else if (bits >= fromBits)
            {
                throw new Exception("illegal zero padding");
            }
            else if (((acc << (int)(toBits - bits)) & maxv) != 0)
            {
                throw new Exception("non-zero padding");
            }
            return ret.ToArray();
        }

        public static (string Hrp, int[] Data) Decode(string addr)
        {
            (string hrp, int[] data) = Bech32.Decode(addr);
            int[] res = ConvertBits(data, 5, 8, false);
            return (hrp, res);
        }

        public static string Encode(string hrp, int[] data)
        {
            int[] bits = ConvertBits(data, 8, 5, true);
            string ret = Bech32.Encode(hrp, bits);
            return ret;
        }
    }
}
