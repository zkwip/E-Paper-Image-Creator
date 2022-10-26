using Cocona;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Dynamic;
using System.IO;

namespace Zkwip.EPIC.Visualizer;

public class Visualizer
{
    static void Main(string[] args) => CoconaApp.Run(Visualize, args);

    static int Visualize(string file, string output)
    {
        try
        {
            if (!File.Exists(file))
                throw new Exception("The provided file does not exist");

            var content = File.ReadAllText(file);

            Bitmap bitmap = BuildImage(content, 400, 300, Profiles.RedBlack, Color.White);

            bitmap.Save(output);

            return 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            return 1;
        }
    }

    private static Bitmap BuildImage(string content, int width, int height, Channel[] channels, Color background)
    {
        Bitmap bitmap = new Bitmap(width, height);
        int channelCount = channels.Length;

        int counter = 0;

        List<Byte>[] bytes = ExtractBytes(content, channels);  

        foreach (Point p in ImageCreator.Pixels(bitmap.Size))
        {
            bitmap.SetPixel(p.X, p.Y, GetColor(counter, bytes, channels, background));
            counter++;
        }

        return bitmap;
    }

    private static Color GetColor(int counter, List<byte>[] bytes, Channel[] channels, Color background)
    {
        for (int c = 0; c < channels.Length; c++)
        {
            byte b = bytes[c][counter/8];
            if ((b & 1<<(counter%8)) == 1)
                return channels[c].Color;
        }

        return background;
    }

    private static List<byte>[] ExtractBytes(string content, Channel[] channels)
    {
        throw new NotImplementedException();
    }
}