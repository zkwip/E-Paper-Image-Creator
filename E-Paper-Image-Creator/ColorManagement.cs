using System;
using System.Collections.Generic;
using System.Drawing;

namespace Zkwip.EPIC
{
    internal static class ColorManagement
    {

        internal static IEnumerable<Color> PixelColors(Bitmap img, Profile profile)
        {
            foreach (Point p in ImageTraversal.Pixels(profile.Size))
                yield return img.GetPixel(p.X, p.Y);
        }

        internal static int Difference(Color a, Color b) => Math.Abs(a.R - b.R) + Math.Abs(a.G - b.G) + Math.Abs(a.B - b.B);

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