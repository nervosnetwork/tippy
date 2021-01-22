using Isopoh.Cryptography.Blake2b;
using Isopoh.Cryptography.SecureArray;

namespace Ckb.Cryptography
{
    public static class Blake2bHasher
    {
        static readonly byte[] personalization = new byte[] { 99, 107, 98, 45, 100, 101, 102, 97, 117, 108, 116, 45, 104, 97, 115, 104 }; // ckb-default-hash

        public static byte[] ComputeHash(byte[] data)
        {
            Blake2BConfig config = new Blake2BConfig
            {
                Personalization = personalization,
                OutputSizeInBytes = 32
            };
            SecureArrayCall secureArrayCall = default;
            return Blake2B.ComputeHash(data, config, secureArrayCall);
        }
    }
}
