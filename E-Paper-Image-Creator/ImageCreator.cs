using Newtonsoft.Json;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.IO;

namespace Zkwip.EPIC
{
    internal static class ImageCreator
    {
        public static int BuildImage(string file, string? output, string? profileName, bool force)
        {
            try
            {
                var profile = ReadProfile(profileName);
                var img = ReadImageFile(file, profile);
                output ??= GenerateOutputFileName(file, ".h");

                var result = CodeFileGeneration.BuildImageCode(img, profile);

                WriteOutputToFile(output, result, force);

                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 1;
            }
        }

        public static int Extract(string file, string? output, string? profileName, bool force)
        {
            try
            {
                var profile = ReadProfile(profileName);
                string content = GetFileContents(file, "file");
                output ??= GenerateOutputFileName(file, ".png");

                var image = ExtractImageContent(content, 400, 300, profile);

                WriteImageToFile(output, image, force);

                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 1;
            }
        }

        private static string GetFileContents(string file, string desc)
        {
            if (!File.Exists(file))
                throw new Exception($"The provided {desc} does not exist");

            var content = File.ReadAllText(file);
            return content;
        }

        private static void WriteImageToFile(string output, Image bitmap, bool force)
        {
            if (File.Exists(output) && !force)
                throw new Exception($"The output file \"{output}\" already exist");

            bitmap.Save(output);
        }

        private static void WriteOutputToFile(string outfile, string contents, bool force)
        {
            if (File.Exists(outfile) && !force)
                throw new Exception($"The output file \"{outfile}\" already exist");

            File.WriteAllText(outfile, contents);
        }

        private static Image<Rgb24> ReadImageFile(string file, Profile profile)
        {
            if (!File.Exists(file))
                throw new Exception("The provided file does not exist");

            var img = Image.Load<Rgb24>(file);

            if (img.Size().IsEmpty)
                throw new Exception("The provided file is not an image file");

            if (img.Width < profile.Width || img.Height < profile.Height)
                throw new Exception("The image is smaller than the target");

            return img;
        }

        private static Profile ReadProfile(string? profile)
        {
            profile ??= "blackwhitered";
            var profileFile = $"Profiles/{profile}.json";

            var profileText = GetFileContents(profileFile, "profile");

            return JsonConvert.DeserializeObject<Profile>(profileText);
        }

        private static Image ExtractImageContent(string content, int width, int height, Profile profile)
        {
            var bitmap = new Image<Rgb24>(width, height);
            var channels = CodeFileReader.ExtractDataChannels(content, profile);

            foreach (Point p in profile.Pixels())
            {
                bitmap[p.X, p.Y] = profile.GetColorFromChannels(p.X, p.Y, channels);
            }

            return bitmap;
        }

        private static string GenerateOutputFileName(string file, string extension)
        {
            if (!file.Contains('.'))
                return file + extension;

            return file[..file.LastIndexOf('.')] + extension;
        }
    }
}