using SixLabors.ImageSharp.PixelFormats;

namespace Zkwip.EPIC;

public struct Swatch
{
    public byte R;
    public byte G;
    public byte B;
    public bool[] Bits;

    public static implicit operator Rgb24(Swatch s) => new(s.R, s.G, s.B);
}
