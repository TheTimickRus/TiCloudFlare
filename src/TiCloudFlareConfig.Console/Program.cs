using Spectre.Console;
using Spectre.Console.Cli;
using TiCloudFlareConfig.Console;
using TiCloudFlareConfig.Console.Libs;

var app = new CommandApp<CloudFlareCommand>();
app.Configure(configurator =>
{
    configurator.Settings.ApplicationName = Constants.Titles.ExecutableName;
    configurator.Settings.ApplicationVersion = Constants.Titles.Version;
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