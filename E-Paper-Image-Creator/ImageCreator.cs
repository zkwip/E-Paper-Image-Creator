using Newtonsoft.Json;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.IO;

namespace Zkwip.EPIC
{
    internal static class ImageCreator
    {
        public static void BuildImage(string file, string? output, string? profileName, bool force, bool disableProgmem, int rotate)
        {
            var profile = ReadProfile(profileName, rotate);
            var img = ReadImageFile(file, profile);
            output ??= GenerateOutputFileName(file, ".cpp");

            var code = new CodeFile(profile, profile.IteratePixels(img)).BuildImageCode(disableProgmem);

            WriteOutputToFile(output, code, force);
        }

        public static void ValidateProfile(string profileName)
        {
            var profile = ReadProfile(profileName, 0);

            if (profile.Validate())
                Console.WriteLine("Profile appears valid");
        }

        public static void Extract(string file, string? output, string? profileName, bool force, int rotate)
        {
            var profile = ReadProfile(profileName, rotate);
            string content = GetFileContents(file, "file");
            output ??= GenerateOutputFileName(file, ".png");

            var code = new CodeFile(profile, content);
            var image = profile.WriteToImage(code.GetAllPixels());

            WriteImageToFile(output, image, force);
        }

        private static string GetFileContents(string file, string desc)
        {
            if (!File.Exists(file))
                throw new EpicSettingsException($"The provided {desc} does not exist");

            var content = File.ReadAllText(file);
            return content;
        }

        private static void WriteImageToFile(string filename, Image bitmap, bool force)
        {
            if (File.Exists(filename) && !force)
                throw new EpicSettingsException($"The output file \"{filename}\" already exist");

            new FileInfo(filename).Directory!.Create();
            bitmap.Save(filename);
        }

        private static void WriteOutputToFile(string outfile, string contents, bool force)
        {
            if (File.Exists(outfile) && !force)
                throw new EpicSettingsException($"The output file \"{outfile}\" already exist");

            File.WriteAllText(outfile, contents);
        }

        private static Image<Rgb24> ReadImageFile(string file, Profile profile)
        {
            if (!File.Exists(file))
                throw new EpicSettingsException("The provided file does not exist");

            var img = Image.Load<Rgb24>(file);

            if (img.Size().IsEmpty)
                throw new EpicSettingsException("The provided file is not an image file");

            if (img.Width < profile.Width || img.Height < profile.Height)
                throw new EpicSettingsException("The image is smaller than the target");

            return img;
        }

        private static Profile ReadProfile(string? profileName, int rotate)
        {
            profileName ??= "kwr_400_300";
            var profileFile = $"Profiles/{profileName}.json";

            var profileText = GetFileContents(profileFile, "profile");
            try
            {
                var profile = JsonConvert.DeserializeObject<Profile>(
                    profileText, 
                    new JsonSerializerSettings() { 
                        MissingMemberHandling = MissingMemberHandling.Error
                });

                profile.Rotate += rotate;
                return profile;
            }
            catch (JsonException ex)
            {
                throw new ProfileValidationException($"Failed to read profile: {ex.Message}", ex);
            }
        }

        private static string GenerateOutputFileName(string file, string extension)
        {
            if (!file.Contains('.'))
                return file + extension;

            return file[..file.LastIndexOf('.')] + extension;
        }
    }
}