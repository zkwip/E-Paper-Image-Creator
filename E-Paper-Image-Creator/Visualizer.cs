using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace Zkwip.EPIC;

public class Visualizer
{
    internal static int Visualize(string file, string output)
    {
        try
        {
            if (!File.Exists(file))
                throw new Exception("The provided file does not exist");

            var content = File.ReadAllText(file);
            var bitmap = BuildImage(content, 400, 300, StandardProfiles.BlackWhiteRed);

            bitmap.Save(output);

            return 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return 1;
        }
    }

    private static Bitmap BuildImage(string content, int width, int height, Profile profile)
    {
        var bitmap = new Bitmap(width, height);
        var counter = 0;

        List<Byte>[] bytes = CodeFileReader.ExtractBytes(content, profile);  

        foreach (Point p in ImageTraversal.Pixels(bitmap.Size))
        {
            bitmap.SetPixel(p.X, p.Y, CodeFileReader.GetColor(counter, bytes, profile));
            counter++;
        }

        return bitmap;
    }
}