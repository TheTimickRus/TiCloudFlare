using Spectre.Console;
using Spectre.Console.Cli;
using TiCloudFlareConfig.Console;
using TiCloudFlareConfig.Console.Libs;

var app = new CommandApp<CloudFlareCommand>();
app.Configure(configurator =>
{
    configurator.Settings.ApplicationName = "ImageResize.exe";
    configurator.Settings.ApplicationVersion = "v.1.5.2 (09.07.2022)";
    configurator.Settings.ExceptionHandler += ex => 
    {
        AnsiConsoleLib.ShowFiglet(Constants.Titles.VeryShortTitle, Justify.Center, Constants.Colors.ErrorColor);
        AnsiConsole.MarkupLine("\n> [bold red]A fatal error has occurred in the operation of the program![/]\n");
        AnsiConsole.WriteException(ex, ExceptionFormats.ShortenEverything);
        
        AnsiConsole.Console.Input.ReadKey(true);
        return -1;
    };
});

app.Run(args);