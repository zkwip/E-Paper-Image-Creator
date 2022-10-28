using Cocona;
using System;
using System.Collections.Generic;
using System.Drawing;
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

            Bitmap bitmap = BuildImage(content, 400, 300, Profiles.BlackWhiteRed);

            bitmap.Save(output);

            return 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            return 1;
        }
    }

    private static Bitmap BuildImage(string content, int width, int height, Profile profile)
    {
        var bitmap = new Bitmap(width, height);

        int counter = 0;

        List<Byte>[] bytes = ExtractBytes(content, profile);  

        foreach (Point p in ImageCreator.Pixels(bitmap.Size))
        {
            bitmap.SetPixel(p.X, p.Y, GetColor(counter, bytes, profile));
            counter++;
        }

        return bitmap;
    }

    private static Color GetColor(int counter, List<byte>[] bytes, Profile profile)
    {
        for (int c = 0; c < profile.Channels.Length; c++)
        {
            byte b = bytes[c][counter/8];
            int bit = counter % 8;

            if (profile.BigEndian) bit = 7 - bit;

            if ((b & 1<<bit) == 0)
                return profile.Channels[c].Color;
        }

        return profile.Background;
    }

    private static List<byte>[] ExtractBytes(string content, Profile profile)
    {
        List<byte>[] bytes = new List<byte>[3];

        const string arrayPrefix = "const unsigned char";
        const string arraySuffix = "[] = {";
        const string arrayClosure = "};";

        var cursor = 0;

        foreach (Channel _ in profile.Channels)
        {
            cursor = content.IndexOf(arrayPrefix, cursor);
            if (cursor == -1)
                break;

            cursor += arrayPrefix.Length;

            var nameEnd = content.IndexOf(arraySuffix, cursor);
            var name = content[cursor..nameEnd].Trim();
            cursor = nameEnd + arraySuffix.Length;

            var arrayEnd = content.IndexOf(arrayClosure, cursor);
            string arrayData = content[cursor..arrayEnd];
            cursor = nameEnd + arrayClosure.Length;

            for (int c = 0; c < profile.Channels.Length; c++)
            {
                if (name == profile.Channels[c].CName)
                {
                    Console.WriteLine($"Found channel {name} with data length of {arrayData.Length} characters.");
                    bytes[c] = ExtractByteArray(arrayData);
                }
            }
        }

        CheckCount(profile, bytes);
        return bytes;
    }

    private static void CheckCount(Profile profile, List<byte>[] bytes)
    {
        var expectedCount = profile.Width * profile.Height / 8;
        if (profile.Width * profile.Height % 8 != 0) expectedCount++;

        for (int c = 0; c < profile.Channels.Length; c++)
        {
            if (bytes[c].Count == 0)
                throw new Exception($"Channel {profile.Channels[c].CName} not found");

            if (bytes[c].Count != expectedCount)
                throw new Exception($"Channel {profile.Channels[c].CName} has the wrong number of bytes: {bytes[c].Count} instead of the expected {expectedCount}");
        }
    }

    private static List<byte> ExtractByteArray(string v)
    {
        v = SkipComments(v);
        var bytes = new List<byte>();
        var cursor = 0;
        var count = 0;

        while (true)
        {
            count++;
            cursor = v.IndexOf("0x", cursor);

            if (cursor == -1) 
                break;

            cursor += 2;

            if (cursor >= v.Length)
                break;

            string code = v.Substring(cursor, 2);

            bytes.Add(Convert.ToByte(code, 16));
            cursor += 2;
        }
        
        return bytes;
    }

    private static string SkipComments(string input)
    {
        input = input.ToLower();
        var cursor = 0;

        while(true)
        {
            if (cursor >= input.Length - 2)
                break;

            if (input.Substring(cursor, 2) == "//")
            {
                input = Slice(input, cursor, input.IndexOf("\n", cursor));
                continue;
            }

            if (input.Substring(cursor, 2) == "/*")
            {
                input = Slice(input, cursor, input.IndexOf("*/", cursor));
                continue;
            }

            cursor++;
        }

        return input;
    }

    private static string Slice(string input, int firstCut, int secondCut)
    {
        if (secondCut < 0 ) secondCut += input.Length;
        return input[..firstCut] + input[secondCut..];
    }
}