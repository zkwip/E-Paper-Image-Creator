using System;
using System.Collections.Generic;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Zkwip.EPIC
{
    internal static class ColorManagement
    {

        internal static IEnumerable<TPixel> PixelColors<TPixel>(Image<TPixel> img, Profile profile) where TPixel : unmanaged, IPixel<TPixel>
        {
            foreach (Point p in ImageTraversal.Pixels(profile.Size))
                yield return img[p.X, p.Y];
        }

        internal static int Difference(Rgb24 a, Rgb24 b) => Math.Abs(a.R - b.R) + Math.Abs(a.G - b.G) + Math.Abs(a.B - b.B);

        internal static bool[] Map(Color pixel, Channel[] channels)
        {
            bool[] map = new bool[channels.Length];

            for (int i = 0; i < channels.Length; i++)
            {
                Channel c = channels[i];
                if (Difference(pixel, c.Color) < c.Distance)
                {
                    map[i] = true;
                    return map;
                }
            }

            return map;
        }
    }
}