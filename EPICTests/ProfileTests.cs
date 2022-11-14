using FluentAssertions;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;

namespace Zkwip.EPIC.Tests
{
    public class ProfileTests
    {
        public readonly static Swatch[] _palette = new Swatch[] {
            new() {
                R= 0,
                G= 0,
                B= 0,
                Bits = new[] { false, false}
            },
            new() {
                R= 255,
                G= 255,
                B= 255,
                Bits = new[] { true, false}
            },
            new() {
                R= 255,
                G= 0,
                B= 0,
                Bits = new[] { false, true}
            }
        };

        public readonly static Profile InterleavedProfile = new()
        {
            BigEndian = true,
            BlockNames = new[]
                {
                    "IMAGE_DATA"
                },
            Width = 2,
            Height = 2,
            Channels = 2,
            FlipHorizontal = false,
            FlipVertical = false,
            Interleaved = true,
            Palette = _palette
        };

        public readonly static Profile SequentialProfile = new()
        {
            BigEndian = true,
            BlockNames = new[]
                {
                    "IMAGE_BLACK",
                    "IMAGE_RED"
                },
            Width = 2,
            Height = 2,
            Channels = 2,
            FlipHorizontal = false,
            FlipVertical = false,
            Interleaved = false,
            Palette = _palette
        };

        public static IEnumerable<object[]> Palette()
        {
            foreach (var s in _palette)
                yield return new object[] { s };
        }

        [Fact]
        public void Pixels_ShouldReturnAllPixels()
        {
            SequentialProfile.Pixels().Should().HaveElementAt(0, new Point(0, 0));
            SequentialProfile.Pixels().Should().HaveElementAt(1, new Point(1, 0));
            SequentialProfile.Pixels().Should().HaveElementAt(2, new Point(0, 1));
            SequentialProfile.Pixels().Should().HaveElementAt(3, new Point(1, 1));
        }

        [Theory]
        [MemberData(nameof(Palette))]
        public void GetClosestPaletteColor_Should_MatchOnPaletteColors(Swatch s)
        {
            Rgb24 pixel = new Rgb24(s.R, s.G, s.B);
            SequentialProfile.GetClosestPaletteColor(pixel).Should().BeEquivalentTo(s.Bits);
        }
    }
}