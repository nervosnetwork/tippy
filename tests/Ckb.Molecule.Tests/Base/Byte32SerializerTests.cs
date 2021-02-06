using Ckb.Molecule.Base;
using Xunit;

namespace Ckb.Molecule.Tests.Base
{
    public class Byte32SerializerTests
    {
        private readonly string Hex = "0xe79f3207ea4980b7fed79956d5934249ceac4751a4fae01a0f7c4a96884bc4e3";
        private readonly byte[] Expected = new byte[] { 231, 159, 50, 7, 234, 73, 128, 183, 254, 215, 153, 86, 213, 147, 66, 73, 206, 172, 71, 81, 164, 250, 224, 26, 15, 124, 74, 150, 136, 75, 196, 227 };

        [Fact]
        public void TestWithPrefix()
        {
            Assert.Equal(new Byte32Serializer(Hex).Serialize(), Expected);
        }

        [Fact]
        public void TestWithoutPrefix()
        {
            Assert.Equal(new Byte32Serializer(Hex.Remove(0, 2)).Serialize(), Expected);
        }

        [Fact]
        public void TestTooLong()
        {
            Assert.Throws<System.Exception>(() => new Byte32Serializer(Hex + "01").Serialize());
        }

        [Fact]
        public void TestTooShort()
        {
            Assert.Throws<System.Exception>(() => new Byte32Serializer(Hex.Remove(0, 4)).Serialize());
        }
    }
}
