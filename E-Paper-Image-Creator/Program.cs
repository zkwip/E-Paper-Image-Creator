using Cocona;

namespace Zkwip.EPIC;

public class Program
{
    private static void Main(string[] args)
    {
        var app = CoconaApp.Create();

        app.AddCommand("build", EPaperImageCreator.Epic);
        app.AddCommand("extract", Visualizer.Visualize);
        app.Run();
    }
}
