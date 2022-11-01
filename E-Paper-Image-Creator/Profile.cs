using System.Drawing;

namespace Zkwip.EPIC;

public struct Profile
{
    // Alignment
    public int Width;
    public int Height;
    public bool BigEndian;

    public bool FlipHorizontal;
    public bool FlipVertical;

    // Colors
    public Channel[] Channels;
    public Color Background;

    // Properties
    public Size Size => new(Width, Height);
}
