using System;
using System.Drawing;
using System.IO;

namespace Zkwip.EPIC;
public class ImageBuilder
{

    internal static int Epic(string file, string? output, bool force)
    {
        try
        {
            Profile profile = StandardProfiles.BlackWhiteRed;

            if (!File.Exists(file))
                throw new Exception("The provided file does not exist");

            var img = new Bitmap(file);

            if (img.Size.IsEmpty)
                throw new Exception("The provided file is not an image file");

            if (img.Width < profile.Width || img.Height < profile.Height)
                throw new Exception("The image is smaller than the target");

            string result = CodeFileGeneration.BuildImageCode(img, profile);


            if (output is null)
                output = file[..file.LastIndexOf('.')] + ".h";

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
}