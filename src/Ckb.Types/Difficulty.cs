using System.Numerics;

namespace Ckb.Types
{
    public class Difficulty
    {
        // UInt256
        public static (BigInteger, bool) CompactToTarget(uint compact)
        {
            uint exponent = compact >> 24;
            BigInteger mantissa = compact & 0x00ff_ffff;

            BigInteger ret;
            if (exponent <= 3)
            {
                mantissa >>= 8 * (3 - (int)exponent);
                ret = mantissa;
            }
            else
            {
                ret = mantissa;
                ret <<= (int)(8 * (exponent - 3));
            }

            bool overflow = mantissa != 0 && (exponent > 32);

            return (ret, overflow);
        }

        public static BigInteger TargetToDifficulty(BigInteger target)
        {
            BigInteger u256MaxValue = BigInteger.Pow(2, 256) - 1;
            BigInteger hspace = Convert.HexToBigInteger("0x10000000000000000000000000000000000000000000000000000000000000000");

            if (target == 0)
            {
                return u256MaxValue;
            }
            return hspace / target;
        }

        public static BigInteger CompactToDifficulty(uint compact)
        {
            var (target, overflow) = CompactToTarget(compact);
            if (target == 0 || overflow)
            {
                return 0;
            }
            return TargetToDifficulty(target);
        }
    }
}
