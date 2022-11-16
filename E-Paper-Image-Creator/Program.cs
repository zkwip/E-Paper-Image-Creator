using Cocona;
using System;

namespace Zkwip.EPIC;

public class Program
{
    private static void Main(string[] _)
    {
        var app = CoconaLiteApp.Create();

        app.AddCommand("build", Build);
        app.AddCommand("extract", Extract);
        app.AddCommand("validate", Validate);
        app.Run();
    }

    private static int RunSafe(Action command)
    {
        try
        {
            command.Invoke();
            return 0;
        }
        catch (EpicSettingsException ex)
        {
            Console.WriteLine(ex.Message);
            return 1;
        }
    }

    private static void Build(
            [Argument("file", Description = "The image file to convert")] string file,
            [Option('o', Description = "Where to write the result")] string? output,
            [Option('p', Description = "The name of the profile to use")] string? profile,
            [Option("override", new char[] {'O'}, Description = "Write even if the output file already exists")] bool force,
            [Option("no-progmem", Description = "Remove the PROGMEM macro for use with devices other than Arduino")] bool progmem
        ) => RunSafe(() => ImageCreator.BuildImage(file, output, profile, force, progmem));

    private static void Extract(
            [Argument("file", Description = "The code file to get the image from")] string file,
            [Option('o', Description = "Where to write the result")] string? output,
            [Option('p', Description = "The name of the profile to use")] string? profile,
            [Option('O', Description = "Write even if the output file already exists", ValueName = "override")] bool force
        ) => RunSafe(() => ImageCreator.Extract(file, output, profile, force));

    private static void Validate(
            [Argument("profile", Description = "The name of the profile to validate")] string profile
        ) => RunSafe(() => ImageCreator.ValidateProfile(profile));

}
