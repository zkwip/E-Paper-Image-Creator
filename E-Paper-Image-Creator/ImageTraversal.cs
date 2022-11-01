using System.Collections.Generic;
using System.Drawing;

namespace Zkwip.EPIC
{
    internal static class ImageTraversal
    {

        public static IEnumerable<Point> Pixels(Size size)
        {
            for (int y = 0; y < size.Height; y++)
            {
                for (int x = 0; x < size.Width; x++)
                {
                    yield return new Point(x, y);
                }
            }
        }

        internal static byte GetBitMask(Profile profile, int counter)
        {
            var position = counter;

            if (profile.BigEndian)
                position = 7 - counter;

            byte bitmask = (byte)(1 << position);
            return bitmask;
        }
    }
}