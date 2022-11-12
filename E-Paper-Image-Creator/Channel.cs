using SixLabors.ImageSharp.PixelFormats;

namespace Zkwip.EPIC;

public struct Channel
{
    public string CName;
    public Rgb24 Color;
    public int Distance;
    public bool StoreInverted;
}
