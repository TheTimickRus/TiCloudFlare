// ReSharper disable RedundantNullableFlowAttribute
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable InvertIf

using System.Diagnostics.CodeAnalysis;
using Ardalis.GuardClauses;
using Spectre.Console;
using Spectre.Console.Cli;
using TiCloudFlareConfig.Console.Enums;
using TiCloudFlareConfig.Console.Libs;
using TiCloudFlareConfig.Shared.LicGenerate;
using TiCloudFlareConfig.Shared.WireGuardConfig;
using TiCloudFlareConfig.Shared.WireGuardConfig.Models;

namespace TiCloudFlareConfig.Console;

public class CloudFlareCommand : Command<CloudFlareCommand.Settings>
{
    public sealed class Settings : CommandSettings
    { }

    private KeysResponse? _keysResponse;
    private WireGuardConfigParams? _configParams;
    private WireGuardConfigResponse? _configResponse;
    
    public override int Execute([NotNull] CommandContext context, [NotNull] Settings settings)
    {
        System.Console.Title = Constants.Titles.VeryShortTitle;
        AnsiConsoleLib.ShowFiglet(Constants.Titles.VeryShortTitle, Justify.Left, Constants.Colors.MainColor);
        AnsiConsoleLib.ShowRule(Constants.Titles.FullTitle, Justify.Right, Constants.Colors.MainColor);

        WireGuardConfig.ExtractResources();
        
        _keysResponse = WireGuardConfig.FetchKeys();
        _configParams = new WireGuardConfigParams
        {
            IsLicGenerate = AnsiConsole.Prompt(
                new SelectionPrompt<bool>()
                    .Title(" Generate a WARP+ license?")
                    .AddChoices(true, false)),
            EndPoint = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title(" Please select ENDPOINT:")
                    .AddChoices(_keysResponse.EndPoints)),
            Port = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title(" Please select PORT:")
                    .AddChoices(_keysResponse.Ports)),
            Mtu = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title(" Please select MTU:")
                    .AddChoices(_keysResponse.MTU))
        };
        
        AnsiConsole.Status()
            .Spinner(Spinner.Known.Clock)
            .Start("Configuration files are being created, please wait...", StatusAction);

        AnsiConsole.Console.Clear();
        AnsiConsoleLib.ShowFiglet(Constants.Titles.VeryShortTitle, Justify.Left, Constants.Colors.MainColor);
        AnsiConsoleLib.ShowRule(Constants.Titles.FullTitle, Justify.Right, Constants.Colors.MainColor);
        
        var fileName = AnsiConsole.Prompt(
            new TextPrompt<string>(" Укажите имя файла БЕЗ расширения")
                .DefaultValue("TiCloudFlare")
                .AllowEmpty());
        
        AnsiConsole.WriteLine();
        
        var type = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title(" Save As:")
                .AddChoices(".config | .toml", ".zip"));

        switch (type)
        {
            case ".config | .toml":
                WireGuardConfig.SaveAsConfig(_configResponse, Environment.CurrentDirectory, fileName);
                break;
            case ".zip":
                WireGuardConfig.SaveAsZip(_configResponse, Environment.CurrentDirectory, fileName);
                break;
        }
        
        return ExitMessage(ReturnStatusType.Success);
    }
    
    private void StatusAction(StatusContext ctx)
    {
        Guard.Against.Null(_configParams);
        Guard.Against.Null(_keysResponse);
        
        if (_configParams.IsLicGenerate)
        {
            AnsiConsoleLib.WriteConsoleLog("WARP+ license-key generation...");
            _configParams.License = new GenerateLicenseService(_keysResponse.WarpKeys).Generate()?.License;
        }
        
        AnsiConsoleLib.WriteConsoleLog(_configParams.License is null 
            ? "WARP account registration..."
            : "WARP+ account registration...");
        _configResponse = WireGuardConfig.Register(_configParams);
    }

    private static int ExitMessage(ReturnStatusType statusType)
    {
        AnsiConsole.WriteLine();
        
        switch (statusType)
        {
            case ReturnStatusType.Success:
                AnsiConsoleLib.ShowRule(
                    "The program has been successfully completed! /* Press any key to shut down */", 
                    Justify.Center, 
                    Constants.Colors.SuccessColor);
                break;
            case ReturnStatusType.Error:
                AnsiConsoleLib.ShowRule(
                    "The program has terminated with an error! /* Press any key to shut down */", 
                    Justify.Center, 
                    Constants.Colors.ErrorColor);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(statusType), statusType, null);
        }
        
        AnsiConsole.Console.Input.ReadKey(true);
        return (int)statusType;
    }
}