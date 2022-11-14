﻿using FluentAssertions;

namespace Zkwip.EPIC.Tests
{
    public class OutputBlockTests
    {
        const string ArrayLiteral = "const unsigned char henk[] = {0x01,0x00,0xFF,0x00};";
        private readonly OutputBlock _sut;

        public OutputBlockTests()
        {
            int cursor = 0;
            _sut = OutputBlock.FromText(ref cursor, ArrayLiteral, 4, true);
        }

        [Fact]
        public void BigEndianOutputBlockFromTextTest()
        {
            int cursor = 0;

            var block = OutputBlock.FromText(ref cursor, ArrayLiteral, 4, true);

            block.Should().NotBeNull();
            block.ByteCount.Should().Be(4);
            block.Name.Should().BeEquivalentTo("henk");
            block.GetBit(0).Should().BeFalse();
            block.GetBit(7).Should().BeTrue();
        }

        [Fact]
        public void LittleEndianOutputBlockFromTextTest()
        {
            int cursor = 0;

            var block = OutputBlock.FromText(ref cursor, ArrayLiteral, 4, false);

            block.Should().NotBeNull();
            block.ByteCount.Should().Be(4);
            block.Name.Should().BeEquivalentTo("henk");
            block.GetBit(7).Should().BeFalse();
            block.GetBit(0).Should().BeTrue();
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