using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

[assembly: InternalsVisibleTo("Ckb.Address.Tests")]
namespace Ckb.Address
{
    internal class Bech32
    {
        static readonly string CHARSET = "qpzry9x8gf2tvdw0s3jn54khce6mua7l";

        static readonly int[] GENERATOR = new int[] { 0x3b6a57b2, 0x26508e6d, 0x1ea119fa, 0x3d4233dd, 0x2a1462b3 };

        static int Polymod(int[] values)
        {
            int chk = 1;

            foreach (int v in values)
            {
                int top = chk >> 25;
                chk = (chk & 0x1ffffff) << 5 ^ v;
                for (int i = 0; i < 5; i++)
                {
                    if (((top >> i) & 1) == 1)
                    {
                        chk ^= GENERATOR[i];
                    }
                }
            }

            return chk;
        }

        static int[] HrpExpand(string hrp)
        {
            List<int> ret = new List<int>();
            foreach (char c in hrp)
            {
                ret.Add(c >> 5);
            }
            ret.Add(0);
            foreach (char c in hrp)
            {
                ret.Add(c & 31);
            }
            return ret.ToArray();
        }

        static bool VerifyChecksum(string hrp, int[] data)
        {
            return Polymod(HrpExpand(hrp).Concat(data).ToArray()) == 1;
        }

        static int[] CreateChecksum(string hrp, int[] data)
        {
            int[] values = HrpExpand(hrp).Concat(data).Concat(
                new int[] { 0, 0, 0, 0, 0, 0 }).ToArray();
            int polymod = Polymod(values) ^ 1;
            int[] ret = new int[6];
            for (int i = 0; i < ret.Length; i++)
            {
                ret[i] = (polymod >> 5 * (5 - i)) & 31;
            }
            return ret;
        }

        internal static string Encode(string hrp, int[] data)
        {
            if (hrp.Length < 1)
            {
                throw new Exception("invalid hrp");
            }

            foreach (char c in hrp)
            {
                if (c < 33 || c > 126)
                {
                    throw new Exception("too long");
                }
            }

            if (hrp.ToUpper() != hrp && hrp.ToLower() != hrp)
            {
                throw new Exception("mix case");
            }

            bool lower = hrp.ToLower() == hrp;
            hrp = hrp.ToLower();
            int[] combined = data.Concat(CreateChecksum(hrp, data)).ToArray();

            StringBuilder ret = new StringBuilder();
            ret.Append(hrp);
            ret.Append('1');

            foreach (int p in combined)
            {
                if (p < 0 || p > CHARSET.Length)
                {
                    throw new Exception("invalid data");
                }
                ret.Append(CHARSET[p]);
            }

            if (lower)
            {
                return ret.ToString();
            }
            return ret.ToString().ToUpper();
        }

        internal static (string Hrp, int[] Data) Decode(string bechString)
        {
            if (bechString.ToLower() != bechString && bechString.ToUpper() != bechString)
            {
                throw new Exception("mix case");
            }

            bool lower = bechString.ToLower() == bechString;

            bechString = bechString.ToLower();
            int pos = bechString.LastIndexOf("1");
            if (pos < 1 || pos + 7 > bechString.Length)
            {
                throw new Exception("separator '1' at invalid position");
            }

            string hrp = bechString.Substring(0, pos);
            foreach (char c in hrp)
            {
                if (c < 33 || c > 126)
                {
                    throw new Exception("invalid character human-readable part");
                }
            }
            List<int> data = new List<int>();
            for (int p = pos + 1; p < bechString.Length; p++)
            {
                int d = CHARSET.IndexOf(bechString[p]);
                if (d == -1)
                {
                    throw new Exception("invalid character data part");
                }
                data.Add(d);
            }
            if (!VerifyChecksum(hrp, data.ToArray()))
            {
                throw new Exception("invalid checksum");
            }
            if (!lower)
            {
                return (hrp.ToUpper(), data.Take(data.Count - 6).ToArray());
            }
            return (hrp, data.Take(data.Count - 6).ToArray());
        }
    }
}
