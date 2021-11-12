using System;
using Cryptography.ECDSA;

namespace Ckb.Cryptography
{
    public static class Secp256k1
    {
        public static byte[] PrivateKeyToPublicKey(byte[] privateKey, bool compressed)
        {
            return Secp256K1Manager.GetPublicKey(privateKey, compressed);
        }

        public static string PrivateKeyToPublicKey(string privateKey, bool compressed)
        {
            var pk = HexStringToBytes(privateKey);
            return BytesToHexString(PrivateKeyToPublicKey(pk, compressed));
        }

        private static byte[] HexStringToBytes(string hex)
        {
            string hexWithoutPrefix = hex.StartsWith("0x") ? hex.Remove(0, 2) : hex;
            int NumberChars = hexWithoutPrefix.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
            {
                bytes[i / 2] = System.Convert.ToByte(hexWithoutPrefix.Substring(i, 2), 16);
            }
            return bytes;
        }

        private static string BytesToHexString(byte[] bytes)
        {
            return "0x" + BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }
    }
}
