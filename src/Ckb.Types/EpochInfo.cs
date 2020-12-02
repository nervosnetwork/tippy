using System;

namespace Ckb.Types
{
    public class EpochInfo
    {
        public UInt64 Length { get; set; }
        public UInt64 Index { get; set; }
        public UInt64 Number { get; set; }

        public EpochInfo(UInt64 number, UInt64 length, UInt64 index)
        {
            Number = number;
            Length = length;
            Index = index;
        }

        public static EpochInfo Parse(UInt64 epoch)
        {
            UInt64 length = (epoch >> 40) & 0xffff;
            UInt64 index = (epoch >> 24) & 0xffff;
            UInt64 number = epoch & 0xffffff;

            return new EpochInfo(number, length, index);
        }
    }
}
