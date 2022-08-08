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
using TiCloudFlareConfig.Shared.LicGenerate.Models.Responses;
using TiCloudFlareConfig.Shared.WireGuardConfig;
using TiCloudFlareConfig.Shared.WireGuardConfig.Models;

namespace TiCloudFlareConfig.Console;

public class CloudFlareCommand : Command<CloudFlareCommand.Settings>
{
    public sealed class Settings : CommandSettings
    { }

    private KeysResponse? _keysResponse;
    private WireGuardConfigParams? _configParams;
    
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
        var configResponse = WireGuardConfig.Register(_configParams);
        
        WireGuardConfig.CreateArchive(configResponse, $"\\{Path.GetFileNameWithoutExtension(configResponse.FileToml)}.zip");
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