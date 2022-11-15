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
        public void OutputBlockFromText_Should_ParseCorrectly()
        {
            _sut.Should().NotBeNull();
            _sut.Name.Should().BeEquivalentTo("henk");
            _sut.GetBit(0).Should().BeFalse();
            _sut.GetBit(7).Should().BeTrue();
            _sut.GetBit(31).Should().BeFalse();

            int cursor = 0;

            var block = OutputBlock.FromText(ref cursor, ArrayLiteral, 32, false);

            block.Should().NotBeNull();
            block.Name.Should().BeEquivalentTo("henk");
            block.GetBit(0).Should().BeTrue();
            block.GetBit(7).Should().BeFalse();
            block.GetBit(31).Should().BeFalse();
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
        public void SetBit_Should_SetBitsCorrectly(int index, bool value)
        {
            _sut.SetBit(index, value);

            _sut.GetBit(index).Should().Be(value);

            _sut.SetBit(index, !value);

            _sut.GetBit(index).Should().Be(!value);
        }
    }
}