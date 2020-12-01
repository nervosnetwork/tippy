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
            var pk = Convert.FromHexString(privateKey);
            return Convert.ToHexString(PrivateKeyToPublicKey(pk, compressed)).ToLower();
        }
    }
}
