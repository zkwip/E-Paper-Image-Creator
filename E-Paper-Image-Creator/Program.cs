using Cocona;
using System;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Zkwip.EPIC;

public class Program
{
    private static void Main(string[] _)
    {
        var app = CoconaApp.Create();

        app.AddCommand("build", Build);
        app.AddCommand("extract", Extract);
        app.Run();
    }

    private static void Build(
            [Argument("file", Description = "The image file to convert")] string file,
            [Option('o', Description = "Where to write the result")] string? output,
            [Option('O', Description = "Write even if the output file already exists", ValueName = "override")] bool force
        ) => BuildImage(file, output, force);

    private static void Extract(
            [Argument("file", Description = "The code file to get the image from")] string file,
            [Option('o', Description = "Where to write the result")] string output,
            [Option('O', Description = "Write even if the output file already exists", ValueName = "override")] bool force
        ) => Visualize(file, output, force);


    internal static int Visualize(string file, string output, bool force)
    {
        try
        {
            if (!File.Exists(file))
                throw new Exception("The provided file does not exist");

            var content = File.ReadAllText(file);
            var bitmap = ExtractImage(content, 400, 300, StandardProfiles.BlackWhiteRed);

            output ??= GenerateOutputFileName(file, ".png");

            if (File.Exists(output) && !force)
                throw new Exception($"The output file \"{output}\" already exist");

            bitmap.Save(output);

            return 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return 1;
        }
    }

    private static Image ExtractImage(string content, int width, int height, Profile profile)
    {
        var bitmap = new Image<Rgb24>(width, height);
        var bytes = CodeFileReader.ExtractBytes(content, profile);
        var counter = 0;

        foreach (Point p in ImageTraversal.Pixels(bitmap.Size()))
        {
            bitmap[p.X, p.Y] = CodeFileReader.GetColor(counter, bytes, profile);
            counter++;
        }


        return bitmap;
    }

    internal static int BuildImage(string file, string? output, bool force)
    {
        try
        {
            Profile profile = StandardProfiles.BlackWhiteRed;

            if (!File.Exists(file))
                throw new Exception("The provided file does not exist");

            var img = Image.Load<Rgb24>(file);

            if (img.Size().IsEmpty)
                throw new Exception("The provided file is not an image file");

            if (img.Width < profile.Width || img.Height < profile.Height)
                throw new Exception("The image is smaller than the target");

            string result = CodeFileGeneration.BuildImageCode(img, profile);


            output ??= GenerateOutputFileName(file, ".h");

            if (File.Exists(output) && !force)
                throw new Exception($"The output file \"{output}\" already exist");

            File.WriteAllText(output, result);

            return 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return 1;
        }
    }

    private static string GenerateOutputFileName(string file, string extension)
    {
        if (!file.Contains('.'))
            return file + extension;

        return file[..file.LastIndexOf('.')] + extension;
    }
}
