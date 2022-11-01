using Cocona;
using System.IO;

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
        ) => ImageBuilder.Epic(file, output, force);

    private static void Extract(
            [Argument("file", Description = "The code file to get the image from")] string file,
            [Option('o', Description = "Where to write the result")] string output,
            [Option('O', Description = "Write even if the output file already exists", ValueName = "override")] bool force
        ) => Visualizer.Visualize(file, output, force);
}
