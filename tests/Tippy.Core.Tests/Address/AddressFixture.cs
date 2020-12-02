using Types = Ckb.Rpc.Types;

namespace Tippy.Core.Tests.Address
{
    public class Fixture

    {
        public string CodeHash { get; set; }
        public string HashType { get; set; }
        public string Args { get; set; }
        public int? ShortId { get; set; }
        public string TestnetAddress { get; set; }
        public string MainnetAddress { get; set; }

        public Types.Script Script()
        {
            return new Types.Script()
            {
                CodeHash = CodeHash,
                HashType = HashType,
                Args = Args
            };
        }

        public Fixture(
            string codeHash,
            string hashType,
            string args,
            int? shortId,
            string testnetAddress,
            string mainnetAddress)
        {
            CodeHash = codeHash;
            HashType = hashType;
            Args = args;
            ShortId = shortId;
            TestnetAddress = testnetAddress;
            MainnetAddress = mainnetAddress;
        }
    }

    public class Addresses
    {
        public static string TestnetPrefix = "ckt";
        public static string MainnetPrefix = "ckb";

        public static Fixture SecpShortAddress = new(
                "0x9bd7e06f3ecf4be0f2fcd2188b23f1b9fcc88e5d4b65a8637b17723bbda3cce8",
                "type",
                "0x36c329ed630d6ce750712a477543672adab57f4c",
                0,
                "ckt1qyqrdsefa43s6m882pcj53m4gdnj4k440axqswmu83",
                "ckb1qyqrdsefa43s6m882pcj53m4gdnj4k440axqdt9rtd");

        public static Fixture MultisigShortAddress = new(
                "0x5c5069eb0857efc65e1bca0c07df34c31663b3622fd3876c876320fc9634e2a8",
                "type",
                "0x4fb2be2e5d0c1a3b8694f832350a33c1685d477a",
                1,
                "ckt1qyq5lv479ewscx3ms620sv34pgeuz6zagaaqt6f5y5",
                "ckb1qyq5lv479ewscx3ms620sv34pgeuz6zagaaqklhtgg");

        public static Fixture LongTypeAddress = new(
                "0x0000000000000000000000000000000000000000000000000000000000000000",
                "type",
                "0xb39bbc0b3673c7d36450bc14cfcdad2d559c6c64",
                null,
                "ckt1qsqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqpvumhs9nvu786dj9p0q5elx66t24n3kxgkpkap5",
                "ckb1qsqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqpvumhs9nvu786dj9p0q5elx66t24n3kxgmz0sxt");

        public static Fixture LongDataAddress = new(
                "0x9bd7e06f3ecf4be0f2fcd2188b23f1b9fcc88e5d4b65a8637b17723bbda3cce8",
                "data",
                "0x36c329ed630d6ce750712a477543672adab57f4c",
                null,
                "ckt1q2da0cr08m85hc8jlnfp3zer7xulejywt49kt2rr0vthywaa50xwsdkr98kkxrtvuag8z2j8w4pkw2k6k4l5cn3l4jk",
                "ckb1q2da0cr08m85hc8jlnfp3zer7xulejywt49kt2rr0vthywaa50xwsdkr98kkxrtvuag8z2j8w4pkw2k6k4l5c7jxc4f");

        public static Fixture[] GetAll()
        {
            return new Fixture[]
            {
                SecpShortAddress,
                MultisigShortAddress,
                LongTypeAddress,
                LongDataAddress,
            };
        }
    }
}
