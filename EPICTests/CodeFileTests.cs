using FluentAssertions;

namespace Zkwip.EPIC.Tests
{
    public class CodeFileTests 
    {
        // 1100, 0011 and 1010 0101
        const string ExampleString = "const unsigned char IMAGE_RED[] PROGMEM = {0xC0}; const unsigned char IMAGE_BLACK[] PROGMEM = {0x30}; ";
        const string ExampleStringInterleaved = "const unsigned char IMAGE_DATA[] PROGMEM = {0xA5}; ";

        private readonly CodeFile _sut;
        private readonly CodeFile _sut_interleaved;

        public CodeFileTests()
        {
            _sut = new CodeFile(ProfileTests.SequentialProfile, ExampleString);
            _sut_interleaved = new CodeFile(ProfileTests.InterleavedProfile, ExampleStringInterleaved);
        }


        [Fact]
        public void GetAllPixels_Should_GetAllPixels()
        {
            _sut.GetAllPixels().Should().HaveCount(4);
            _sut_interleaved.GetAllPixels().Should().HaveCount(4);
        }

        [Fact]
        public void GetAllPixels_Should_NotThrow()
        {
            var act = () => _sut.GetAllPixels();
            var act2 = () => _sut_interleaved.GetAllPixels();

            act.Should().NotThrow();
            act2.Should().NotThrow();
        }
        [Theory]
        [InlineData(0, new bool[] { true, false})]
        [InlineData(1, new bool[] { true, false })]
        [InlineData(2, new bool[] { false, true })]
        [InlineData(3, new bool[] { false, true })]
        public void GetPixelBits_Should_ReturnAllIndices(int i, bool[] expected)
        {
            var value = _sut.GetPixelBits(i);

            value.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void FromContent_Should_ReadCorrectly_Sequential()
        {
            _sut.GetPixelBits(0).Should().BeEquivalentTo(new bool[] { true, false });
            _sut.GetPixelBits(1).Should().BeEquivalentTo(new bool[] { true, false });
            _sut.GetPixelBits(2).Should().BeEquivalentTo(new bool[] { false, true });
            _sut.GetPixelBits(3).Should().BeEquivalentTo(new bool[] { false, true });
        }

        [Fact]
        public void BuildImageCode_Should_BuildCorrectly_Sequential()
        {
            var text = _sut.BuildImageCode(false);

            var newCodeFile = new CodeFile(ProfileTests.SequentialProfile, text);

            newCodeFile.GetPixelBits(0).Should().BeEquivalentTo(new bool[] { true, false });
            newCodeFile.GetPixelBits(1).Should().BeEquivalentTo(new bool[] { true, false });
            newCodeFile.GetPixelBits(2).Should().BeEquivalentTo(new bool[] { false, true });
            newCodeFile.GetPixelBits(3).Should().BeEquivalentTo(new bool[] { false, true });

            newCodeFile.BuildImageCode(false).Should().BeEquivalentTo(text);
        }

        [Fact]
        public void FromContent_Should_ReadCorrectly_Interleaved()
        {
            _sut_interleaved.GetPixelBits(0).Should().BeEquivalentTo(new bool[] { true, false });
            _sut_interleaved.GetPixelBits(1).Should().BeEquivalentTo(new bool[] { true, false });
            _sut_interleaved.GetPixelBits(2).Should().BeEquivalentTo(new bool[] { false, true });
            _sut_interleaved.GetPixelBits(3).Should().BeEquivalentTo(new bool[] { false, true });
        }

        [Fact]
        public void BuildImageCode_Should_BuildCorrectly_Interleaved()
        {
            var text = _sut_interleaved.BuildImageCode(false);

            var newCodeFile = new CodeFile(ProfileTests.InterleavedProfile, text);

            newCodeFile.GetPixelBits(0).Should().BeEquivalentTo(new bool[] { true, false });
            newCodeFile.GetPixelBits(1).Should().BeEquivalentTo(new bool[] { true, false });
            newCodeFile.GetPixelBits(2).Should().BeEquivalentTo(new bool[] { false, true });
            newCodeFile.GetPixelBits(3).Should().BeEquivalentTo(new bool[] { false, true });

            newCodeFile.BuildImageCode(false).Should().BeEquivalentTo(text);
        }
    }
}