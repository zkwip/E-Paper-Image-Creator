using Newtonsoft.Json;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Zkwip.EPIC;

public struct Profile
{

    // Alignment
    [JsonRequired]
    public int Width;
    [JsonRequired]
    public int Height;

    [JsonRequired]
    public int GroupX;
    [JsonRequired]
    public int GroupY;

    [JsonRequired]
    public int Rotate;
    [JsonRequired]
    public bool FlipHorizontal;
    [JsonRequired]
    public bool FlipVertical;
    [JsonRequired]

    public int Channels;
    [JsonRequired]
    public bool ExplicitSize;
    [JsonRequired]
    public bool MsbFirst;
    [JsonRequired]
    public bool Interleaved;

    // Color Management
    public string[] BlockNames;
    public Swatch[] Palette;

    internal IEnumerable<Point> ImagePixels()
    {
        NormalizeRotation();
        for (int gy = 0; gy < YGroups; gy++)
        {
            var offset_y = gy * GroupY;

            // if (FlipVertical)
            //     offset_y = GroupY * (YGroups - gy - 1);

            for (int gx = 0; gx < XGroups; gx++)
            {
                var offset_x = gx * GroupX;

                // if (FlipHorizontal)
                //     offset_x = GroupX * (XGroups - gx - 1);

                for (int y = 0; y < GroupY; y++)
                {
                    for (int x = 0; x < GroupX; x++)
                    {
                        yield return MapToImageSpace(x + offset_x, y + offset_y);
                    }
                }
            }
        }
    }

    private Point MapToImageSpace(int x, int y)
    {
        if (Rotate < 0 || Rotate >= 4)
            throw new ProfileValidationException("Rotation value out of range");

        bool rotFlipX = Rotate == 2 || Rotate == 3;
        bool rotFlipY = Rotate == 1 || Rotate == 2;

        if (FlipHorizontal ^ rotFlipX)
            x = Width - 1 - x;

        if (FlipVertical ^ rotFlipY)
            y = Height - 1 - y;

        if (Rotate % 2 != 0)
            return new Point(y, x);

        return new Point(x, y);
    }

    private int XGroups => 1 + (Width - 1) / GroupX;
    private int YGroups => 1 + (Height - 1) / GroupY;

    internal int Entries => XGroups * YGroups * GroupX * GroupY;

    internal IEnumerable<bool[]> IteratePixels(Image<Rgb24> img)
    {
        foreach(var imagePixel in ImagePixels())
        {
            if (InsideBounds(imagePixel))
                yield return GetClosestPaletteColor(img[imagePixel.X, imagePixel.Y]);

            yield return Palette[0].Bits;
        }
    }

    private bool InsideBounds(Point pixel)
    {
        if (Rotate % 2 == 0)
            return pixel.X >= 0 && pixel.Y >= 0 && pixel.X < Width && pixel.Y < Height;

        return pixel.X >= 0 && pixel.Y >= 0 && pixel.X < Height && pixel.Y < Width;
    }

    internal Image<Rgb24> WriteToImage(IEnumerable<bool[]> data)
    {
        var bitmap = CreateEmptyBitmap();

        var bitEnumerator = data.GetEnumerator();

        foreach (var imagePixel in ImagePixels())
        {
            if (!bitEnumerator.MoveNext())
                throw new IndexOutOfRangeException("Pixel stream ended before traversing all pixels");

            if (InsideBounds(imagePixel))
                bitmap[imagePixel.X, imagePixel.Y] = GetColorFromChannels(bitEnumerator.Current);
        }

        return bitmap;
    }

    private Image<Rgb24> CreateEmptyBitmap()
    {
        NormalizeRotation();
        if (Rotate % 2 == 0)
            return new Image<Rgb24>(Width, Height);
        
        return new Image<Rgb24>(Height, Width);
    }

    private void NormalizeRotation()
    {
        Rotate = (Rotate + 8) % 4;
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
        if (GroupX <= 0)
            throw new ProfileValidationException("GroupX must be positive");

        if (GroupY <= 0)
            throw new ProfileValidationException("GroupY must be positive");

        if (Width <= 0)
            throw new ProfileValidationException("Width must be positive");

        if (Height <= 0)
            throw new ProfileValidationException("Height must be positive");

        if (Rotate <= -4 || Rotate >= 4)
            throw new ProfileValidationException("Rotation must be between -4 and 4");

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
