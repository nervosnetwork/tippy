using System;
using System.Collections.Generic;
using System.Linq;

namespace Ckb.Molecule.Base
{
    public abstract class BaseSerializer
    {
        public abstract byte[] Header { get; }

        public abstract byte[] Body { get; }

        public byte[] Serialize()
        {
            return new List<byte>(Header).Concat(new List<byte>(Body)).ToArray();
        }
    }

    public abstract class BaseSerializer<T> : BaseSerializer
    {
        protected const uint Uint32Capacity = 4;
        protected T Value;

        public BaseSerializer(T value)
        {
            Value = value;
        }

        public static byte[] HexStringToBytes(string hex)
        {
            string hexWithoutPrefix = hex.StartsWith("0x") ? hex.Remove(0, 2) : hex;
            int NumberChars = hexWithoutPrefix.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hexWithoutPrefix.Substring(i, 2), 16);
            }
            return bytes;
        }

        public static string BytesToHexString(byte[] bytes)
        {
            return BitConverter.ToString(bytes).Replace("-", "");
        }

        public static byte[] UInt32ToLEBytes(uint num)
        {
            if (BitConverter.IsLittleEndian)
            {
                return BitConverter.GetBytes(num);
            }
            return BitConverter.GetBytes(num).Reverse().ToArray();
        }

        public static byte[] UInt64ToLEBytes(ulong num)
        {
            if (BitConverter.IsLittleEndian)
            {
                return BitConverter.GetBytes(num);
            }
            return BitConverter.GetBytes(num).Reverse().ToArray();
        }
    }
}
