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
            [Option("rotate", new char[] { 'r' }, Description = "Rotate the image in steps of 90 degrees")] int rotate,
            [Option("flip-horizontal", Description = "Flip the image horizontally")] bool flipx,
            [Option("flip-vertical", Description = "Flip the image vertically")] bool flipy,
            [Option("no-progmem", Description = "Remove the PROGMEM macro for use with devices other than Arduino")] bool progmem,
            [Option("override", new char[] { 'O' }, Description = "Write even if the output file already exists")] bool force
        ) => RunSafe(() => ImageCreator.BuildImage(file, output, profile, force, progmem, rotate, flipx, flipy));

    private static void Extract(
            [Argument("file", Description = "The code file to get the image from")] string file,
            [Option('o', Description = "Where to write the result")] string? output,
            [Option('p', Description = "The name of the profile to use")] string? profile,
            [Option("rotate", new char[] { 'r' }, Description = "Rotate the image in steps of 90 degrees")] int rotate,
            [Option("flip-horizontal", Description = "Flip the image horizontally")] bool flipx,
            [Option("flip-vertical", Description = "Flip the image vertically")] bool flipy,
            [Option('O', Description = "Write even if the output file already exists", ValueName = "override")] bool force
        ) => RunSafe(() => ImageCreator.Extract(file, output, profile, force, rotate, flipx, flipy));

    private static void Validate(
            [Argument("profile", Description = "The name of the profile to validate")] string profile
        ) => RunSafe(() => ImageCreator.ValidateProfile(profile));

}
