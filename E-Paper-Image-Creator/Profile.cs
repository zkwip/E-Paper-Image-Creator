using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Zkwip.EPIC;

public struct Profile
{
    // Alignment
    public int Width;
    public int Height;
    public bool BigEndian;

    public bool FlipHorizontal;
    public bool FlipVertical;

    public int Channels;
    public bool Interleaved;

    // Color Management
    public string[] BlockNames;
    public Swatch[] Palette;

    public IEnumerable<Point> Pixels()
    {
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                yield return new Point(x, y);
            }
        }
    }

    internal bool[] GetClosestPaletteColor(Rgb24 pixel)
    {
        int min = 1000;
        bool[] map = new bool[Channels];

        foreach (Swatch s in Palette)
        {
            int dif = Difference(s, pixel);
            if (dif < min)
            {
                min = dif;
                s.Bits.CopyTo(map, 0);
            }
        }

        return map;
    }

    internal static int Difference(Rgb24 a, Rgb24 b) => Math.Abs(a.R - b.R) + Math.Abs(a.G - b.G) + Math.Abs(a.B - b.B);

    internal Rgb24 GetColorFromChannels(int x, int y, CodeFile file)
    {
        var colorBits = file.GetBlockPixel(x, y);

        foreach (Swatch s in Palette)
        {
            if (Enumerable.SequenceEqual(colorBits, s.Bits))
                return s;
        }

        throw new EpicSettingsException($"No matching palette color found with bits {DisplayBits(colorBits)}.");
    }

    private static object DisplayBits(bool[] colorBits)
    {
        string s = "[ ";

        foreach (bool b in colorBits)
            s += (b ? 1 : 0) + " ";

        return s + "]";
    }

    internal int OutputBlockLength
    {
        get
        {
            if (Interleaved)
                return (Width * Height * Channels - 1) / 8 + 1;
            return (Width * Height - 1) / 8 + 1;
        }
    }
}
