using Avalonia;
using Avalonia.ReactiveUI;
using System;

namespace Tubes3_SiHashtag;

sealed class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args) {
        if(args.Length == 1 && args[0] == "preprocess") Seeder.PreprocessSidikjari();
        else if(args.Length == 1 && args[0] == "check") Seeder.CheckDatabaseContent();
        else if(args.Length == 2 && args[0] == "seed") Seeder.StartSeeding(args[1]);
        else if(args.Length == 0) BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
        else {
            Console.WriteLine("Invalid argument");
            Console.WriteLine("Run: dotnet run");
            Console.WriteLine("Command: dotnet run <flag>");
            Console.WriteLine("flag: preprocess | seed");
            return;
        }
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace()
            .UseReactiveUI();
}
