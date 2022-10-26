using Cocona;
using System;
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

            Bitmap bitmap = BuildImage(content, 300, 400, Profiles.RedBlack);

            bitmap.Save(output);

            return 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            return 1;
        }
    }

    private static Bitmap BuildImage(string content, int v1, int v2, Channel[] redBlack)
    {
        throw new NotImplementedException();
    }
}