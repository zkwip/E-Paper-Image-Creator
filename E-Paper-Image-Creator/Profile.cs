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

    public bool MsbFirst;
    public int GroupX;
    public int GroupY;

    public bool FlipHorizontal;
    public bool FlipVertical;

    public int Channels;
    public bool Interleaved;
    public int Rotate;

    // Color Management
    public string[] BlockNames;
    public Swatch[] Palette;

    internal IEnumerable<Point> Pixels()
    {
        int groups_x = Width / GroupX;
        int groups_y = Height / GroupY;

        for (int gx = 0; gx < groups_x; gx++)
        {
            var offset_x = gx * GroupX;

            if (FlipHorizontal)
                offset_x = GroupX * (groups_x - gx - 1);

            for (int gy = 0; gy < groups_y; gy++)
            {
                var offset_y = gy * GroupY;

                if (FlipVertical)
                    offset_y = GroupY * (groups_y - gy - 1);

                for (int x = 0; x < GroupX; x++)
                {
                    for (int y = 0; y < GroupY; y++)
                    {
                        yield return new Point(x + offset_x, y + offset_y);
                    }
                }
            }
        }
    }

    internal IEnumerable<bool[]> IteratePixels(Image<Rgb24> img)
    {
        foreach(var pixel in Pixels())
        {
            yield return GetClosestPaletteColor(img[pixel.X, pixel.Y]);
        }
    }

    internal Image<Rgb24> Extract(IEnumerable<bool[]> pixels)
    {
        var bitmap = new Image<Rgb24>(Width, Height);

        var enumerator = pixels.GetEnumerator();

        foreach(var pixel in Pixels())
        {
            if (!enumerator.MoveNext())
                throw new IndexOutOfRangeException("Pixel stream ended before traversing all pixels");

            bitmap[pixel.X, pixel.Y] = GetColorFromChannels(enumerator.Current);
        }

        return bitmap;
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
    
    internal Rgb24 GetColorFromChannels(bool[] colorBits) 
    { 
        foreach (Swatch s in Palette)
        {
            if (Enumerable.SequenceEqual(colorBits, s.Bits))
                return s;
        }

        throw new ProfileMismatchException($"No matching palette color found with bits {DisplayBits(colorBits)}.");
    }

    private static string DisplayBits(bool[] colorBits)
    {
        string s = "[ ";

        foreach (bool b in colorBits)
            s += (b ? 1 : 0) + " ";

        return s + "]";
    }

    internal bool Validate()
    {
        if (Width <= 0)
            throw new ProfileValidationException("Width must be positive");

        if (Height <= 0)
            throw new ProfileValidationException("Height must be positive");

        if (Channels <= 0)
            throw new ProfileValidationException("Channels must be positive");

        if (BlockNames.Length <= 0)
            throw new ProfileValidationException("BlockNames are missing");

        if (Palette.Length <= 0)
            throw new ProfileValidationException("The palette cannot be empty");

        if (Interleaved && BlockNames.Length != 1)
            throw new ProfileValidationException("Interleaved profiles can only have one block name");

        if (!Interleaved && BlockNames.Length != Channels)
            throw new ProfileValidationException("Sequential profiles need to have exactly one block name per channel");

        for(int i=0; i<BlockNames.Length;i++)
        {
            if (BlockNames[i].Contains(' '))
                throw new ProfileValidationException("BlockNames cannot contain spaces");

            for (int j = 0; j < i; j++)
            {
                if (BlockNames[i] == BlockNames[j])
                    throw new ProfileValidationException("BlockNames need to be unique");
            }
        }

        for (int i = 0; i < Palette.Length; i++)
        {
            for (int j = 0; j < i; j++)
            {
                if (Enumerable.SequenceEqual(Palette[i].Bits, Palette[j].Bits))
                    throw new ProfileValidationException($"Two bit values in the palette cannot be to be unique ({i}, {j})");

                if ((Rgb24)Palette[i] == (Rgb24)Palette[j])
                    throw new ProfileValidationException($"Two color values in the palette cannot be to be unique ({i}, {j})");
            }
        }

        return true;
    }
}
