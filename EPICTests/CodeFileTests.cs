using FluentAssertions;

namespace Zkwip.EPIC.Tests
{
    public class CodeFileTests 
    {
        // 1100, 0011 and 1010 0101
        const string ExampleString = "const unsigned char IMAGE_RED[] PROGMEM = {0xC0}; const unsigned char IMAGE_BLACK[] PROGMEM = {0x30}; ";
        const string ExampleStringInterleaved = "const unsigned char IMAGE_DATA[] PROGMEM = {0xA5}; ";

        CodeFile _sut;
        CodeFile _sut_interleaved;

        public CodeFileTests()
        {
            _sut = CodeFile.FromContent(ProfileTests.SequentialProfile, ExampleString);
            _sut_interleaved = CodeFile.FromContent(ProfileTests.InterleavedProfile, ExampleStringInterleaved);
        }


        [Fact]
        public void FromContent_Should_ReadCorrectly_Sequential()
        {
            _sut.GetBlockPixel(0, 0).Should().BeEquivalentTo(new bool[] { true, false });
            _sut.GetBlockPixel(0, 1).Should().BeEquivalentTo(new bool[] { true, false });
            _sut.GetBlockPixel(1, 0).Should().BeEquivalentTo(new bool[] { false, true });
            _sut.GetBlockPixel(1, 1).Should().BeEquivalentTo(new bool[] { false, true });
        }

        [Fact]
        public void BuildImageCode_Should_BuildCorrectly_Sequential()
        {
            var text = _sut.BuildImageCode(false);

            var newCodeFile = CodeFile.FromContent(ProfileTests.SequentialProfile, text);

            newCodeFile.GetBlockPixel(0, 0).Should().BeEquivalentTo(new bool[] { true, false });
            newCodeFile.GetBlockPixel(0, 1).Should().BeEquivalentTo(new bool[] { true, false });
            newCodeFile.GetBlockPixel(1, 0).Should().BeEquivalentTo(new bool[] { false, true });
            newCodeFile.GetBlockPixel(1, 1).Should().BeEquivalentTo(new bool[] { false, true });

            newCodeFile.BuildImageCode(false).Should().BeEquivalentTo(text);
        }

        [Fact]
        public void FromContent_Should_ReadCorrectly_Interleaved()
        {
            _sut_interleaved.GetBlockPixel(0, 0).Should().BeEquivalentTo(new bool[] { true, false });
            _sut_interleaved.GetBlockPixel(0, 1).Should().BeEquivalentTo(new bool[] { true, false });
            _sut_interleaved.GetBlockPixel(1, 0).Should().BeEquivalentTo(new bool[] { false, true });
            _sut_interleaved.GetBlockPixel(1, 1).Should().BeEquivalentTo(new bool[] { false, true });
        }

        [Fact]
        public void BuildImageCode_Should_BuildCorrectly_Interleaved()
        {
            var text = _sut_interleaved.BuildImageCode(false);

            var newCodeFile = CodeFile.FromContent(ProfileTests.InterleavedProfile, text);

            newCodeFile.GetBlockPixel(0, 0).Should().BeEquivalentTo(new bool[] { true, false });
            newCodeFile.GetBlockPixel(0, 1).Should().BeEquivalentTo(new bool[] { true, false });
            newCodeFile.GetBlockPixel(1, 0).Should().BeEquivalentTo(new bool[] { false, true });
            newCodeFile.GetBlockPixel(1, 1).Should().BeEquivalentTo(new bool[] { false, true });

            newCodeFile.BuildImageCode(false).Should().BeEquivalentTo(text);
        }

        [Fact]
        public void SetBlockPixel_Should_SetTheBits()
        {
            _sut.SetBlockPixel(1, 0, new bool[] { false, false });
            _sut_interleaved.SetBlockPixel(1, 0, new bool[] { false, false });

            _sut.GetBlockPixel(1, 0).Should().BeEquivalentTo(new bool[] { false, false });
            _sut_interleaved.GetBlockPixel(1, 0).Should().BeEquivalentTo(new bool[] { false, false });



            _sut.SetBlockPixel(1, 0, new bool[] { true, true });
            _sut_interleaved.SetBlockPixel(1, 0, new bool[] { true, true });

            _sut.GetBlockPixel(1, 0).Should().BeEquivalentTo(new bool[] { true, true });
            _sut_interleaved.GetBlockPixel(1, 0).Should().BeEquivalentTo(new bool[] { true, true });
        }
    }
}