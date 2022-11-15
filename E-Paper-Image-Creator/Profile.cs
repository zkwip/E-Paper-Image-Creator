using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

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

    private static int Difference(Rgb24 a, Rgb24 b) => Math.Abs(a.R - b.R) + Math.Abs(a.G - b.G) + Math.Abs(a.B - b.B);

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

    private static string DisplayBits(bool[] colorBits)
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

    internal bool Validate()
    {
        if (Width <= 0)
            throw new Exception("Width must be positive");

        if (Height <= 0)
            throw new Exception("Height must be positive");

        if (Channels <= 0)
            throw new Exception("Channels must be positive");

        if (BlockNames.Length <= 0)
            throw new Exception("BlockNames are missing");

        if (Palette.Length <= 0)
            throw new Exception("The palette cannot be empty");

        if (Interleaved && BlockNames.Length != 1)
            throw new Exception("Interleaved profiles can only have one block name");

        if (!Interleaved && BlockNames.Length != Channels)
            throw new Exception("Sequential profiles need to have exactly one block name per channel");

        for(int i=0; i<BlockNames.Length;i++)
        {
            if (BlockNames[i].Contains(' '))
                throw new Exception("BlockNames cannot contain spaces");

            for (int j = 0; j < i; j++)
            {
                if (BlockNames[i] == BlockNames[j])
                    throw new Exception("BlockNames need to be unique");
            }
        }

        for (int i = 0; i < Palette.Length; i++)
        {
            for (int j = 0; j < i; j++)
            {
                if (Enumerable.SequenceEqual(Palette[i].Bits, Palette[j].Bits))
                    throw new Exception($"Two bit values in the palette cannot be to be unique ({i}, {j})");

                if ((Rgb24)Palette[i] == (Rgb24)Palette[j])
                    throw new Exception($"Two color values in the palette cannot be to be unique ({i}, {j})");
            }
        }

        return true;
    }
}
