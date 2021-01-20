using Ckb.Molecule.Base;
using Xunit;

namespace Ckb.Molecule.Tests.Base
{
    public class ByteSerializerTests
    {
        [Fact]
        public void TestSerialize()
        {
            Assert.Equal(new ByteSerializer(255).Serialize(), new byte[] { 255 });
            Assert.Equal(new ByteSerializer("255").Serialize(), new byte[] { 255 });
        }

        [Fact]
        public void TestOverflow()
        {
            Assert.Throws<System.OverflowException>(() => new ByteSerializer("256").Serialize());
        }

        [Fact]
        public void TestUnderflow()
        {
            Assert.Throws<System.OverflowException>(() => new ByteSerializer("-1").Serialize());
        }
    }
}
