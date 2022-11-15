using FluentAssertions;

namespace Zkwip.EPIC.Tests
{
    public class OutputBlockTests
    {
        // A 32 bit char[]
        const string ArrayLiteral = "const unsigned char henk[] = {0x01,0x00,0xFF,0x00};";
        private readonly OutputBlock _sut;

        public OutputBlockTests()
        {
            int cursor = 0;
            _sut = OutputBlock.FromText(ref cursor, ArrayLiteral, 32, true);
        }

        [Fact]
        public void OutputBlockFromTextTest_Should_ReadCorrectly()
        {
            _sut.Should().NotBeNull();
            _sut.Name.Should().BeEquivalentTo("henk");
            _sut.GetBit(0).Should().BeFalse();
            _sut.GetBit(7).Should().BeTrue();
            _sut.GetBit(31).Should().BeFalse();
            
            var cursor = 0;
            var block2 = OutputBlock.FromText(ref cursor, ArrayLiteral, 32, false);

            block2.Should().NotBeNull();
            block2.Name.Should().BeEquivalentTo("henk");
            block2.GetBit(0).Should().BeTrue();
            block2.GetBit(7).Should().BeFalse();
            block2.GetBit(31).Should().BeFalse();
        }

        [Theory]
        [InlineData(0, true)]
        [InlineData(0, false)]
        [InlineData(7, true)]
        [InlineData(7, false)]
        [InlineData(8, true)]
        [InlineData(8, false)]
        [InlineData(15, true)]
        [InlineData(15, false)]
        public void SetBitShouldSetABit(int index, bool value)
        {
            _sut.SetBit(index, value);
            _sut.GetBit(index).Should().Be(value);

            _sut.SetBit(index, !value);
            _sut.GetBit(index).Should().Be(!value);
        }
    }
}