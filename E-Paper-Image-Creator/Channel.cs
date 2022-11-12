using SixLabors.ImageSharp.PixelFormats;

namespace Zkwip.EPIC;

public struct Channel
{
    public string CName;
    public int Distance;
    public bool StoreInverted;

    public Rgb24 Color;
}
