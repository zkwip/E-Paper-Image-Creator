using Cocona;
using System.Drawing;

internal class Program
{
    private static void Main(string[] args) => CoconaApp.Run(Epic, args);
    
    private static int Epic([Option("f")] string file, [Option("o")] string? output, [Option("F")] string? format)
    {
        if (!File.Exists(file))
            return StopWithText("The provided file does not exist");

        var img = Image.FromFile(file);

        if (img.Size.IsEmpty)
            return StopWithText("The provided file is not an image file");

        format ??= "krw";

        string result = BuildImageCode(img, format);

        if (output is null)
            Console.Write(result);


        return 0;
    }

    static string BuildImageCode(Image img, string format)
    {
        throw new NotImplementedException();
    }

    static int StopWithText(string text, int val = 1)
    {
        Console.WriteLine(text);
        return val;
    }
}
