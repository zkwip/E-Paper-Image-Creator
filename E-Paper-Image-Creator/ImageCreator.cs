using Newtonsoft.Json;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.IO;

namespace Zkwip.EPIC
{
    internal static class ImageCreator
    {
        internal static int BuildImage(string file, string? output, string? profileName, bool force)
        {
            try
            {
                var profile = GetProfile(profileName);
                var img = GetImageFile(file, profile);
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

        internal static int Visualize(string file, string? output, string? profileName, bool force)
        {
            try
            {
                var profile = GetProfile(profileName);

                Console.WriteLine(JsonConvert.SerializeObject(profile, Formatting.Indented));
                Console.WriteLine(JsonConvert.SerializeObject(StandardProfiles.BlackWhiteRed, Formatting.Indented));

                string content = GetFileContents(file, "file");
                output ??= GenerateOutputFileName(file, ".png");

                var image = ExtractImage(content, 400, 300, profile);

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

        private static Image<Rgb24> GetImageFile(string file, Profile profile)
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

        private static Profile GetProfile(string? profile)
        {
            profile ??= "blackwhitered";
            var profileFile = $"Profiles/{profile}.json";

            var profileText = GetFileContents(profileFile, "profile");

            return JsonConvert.DeserializeObject<Profile>(profileText);
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

        private static string GenerateOutputFileName(string file, string extension)
        {
            if (!file.Contains('.'))
                return file + extension;

            return file[..file.LastIndexOf('.')] + extension;
        }
    }
}