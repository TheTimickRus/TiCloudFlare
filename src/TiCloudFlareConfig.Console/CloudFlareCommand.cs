// ReSharper disable RedundantNullableFlowAttribute
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable InvertIf

using System.Diagnostics.CodeAnalysis;
using Spectre.Console;
using Spectre.Console.Cli;
using TiCloudFlareConfig.Console.Enums;
using TiCloudFlareConfig.Console.Libs;
using TiCloudFlareConfig.Console.Services.LicGenerate;
using TiCloudFlareConfig.Console.Services.LicGenerate.Models.Responses;
using TiCloudFlareConfig.Console.Services.WireGuardConfig;

namespace TiCloudFlareConfig.Console;

public class CloudFlareCommand : Command<CloudFlareCommand.Settings>
{
    public sealed class Settings : CommandSettings
    { }

    private bool _isSuccess;
    
    public override int Execute([NotNull] CommandContext context, [NotNull] Settings settings)
    {
        System.Console.Title = Constants.Titles.VeryShortTitle;
        AnsiConsoleLib.ShowFiglet(Constants.Titles.VeryShortTitle, Justify.Left, Constants.Colors.MainColor);
        AnsiConsoleLib.ShowRule(Constants.Titles.FullTitle, Justify.Right, Constants.Colors.MainColor);

        AnsiConsole.Status()
            .Spinner(Spinner.Known.Clock)
            .Start("Идет создание конфигурации, пожалуйста, подождите...", StatusAction);
        
        return ExitMessage(_isSuccess ? ReturnStatus.Success : ReturnStatus.Error);
    }
    
    private void StatusAction(StatusContext ctx)
    {
        AnsiConsoleLib.WriteConsoleLog("Генерация License-Key...");
        RegAccountResponse? regAccountResponse = null;
        try
        {
            regAccountResponse = new GenerateLicenseService().Generate();
        }
        catch
        { 
            // ignored
        }

        if (regAccountResponse != null)
        {
            AnsiConsoleLib.WriteConsoleLog($"License-Key успешно получен! {regAccountResponse.License}");
            AnsiConsoleLib.WriteConsoleLog("Регистрация WARP+ аккаунта...");
            var registerWithLicense = WireGuardConfig.RegisterWithLicense(regAccountResponse.License);
            
            AnsiConsoleLib.WriteConsoleLog(registerWithLicense 
                ? "Файлы конфигурации успешно созданы!" 
                : "Создание файлов конфигурации завершилось ошибкой...");
            _isSuccess = registerWithLicense;
            return;
        }
        
        AnsiConsoleLib.WriteConsoleLog("Произошла ошибка при получении License-Key...");
        AnsiConsoleLib.WriteConsoleLog("Регистрация WARP аккаунта...");
        var registerWithoutLicense = WireGuardConfig.RegisterWithoutLicense();
        AnsiConsoleLib.WriteConsoleLog(registerWithoutLicense 
            ? "Файлы конфигурации успешно созданы!" 
            : "Создание файлов конфигурации завершилось ошибкой...");
        _isSuccess = registerWithoutLicense;
    }

    private static int ExitMessage(ReturnStatus status)
    {
        AnsiConsole.WriteLine();
        
        switch (status)
        {
            case ReturnStatus.Success:
                AnsiConsoleLib.ShowRule(
                    "Работа программы успешно завершена! /* Нажмите любую клавишу для завершения работы */", 
                    Justify.Center, 
                    Constants.Colors.SuccessColor);
                break;
            case ReturnStatus.Error:
                AnsiConsoleLib.ShowRule(
                    "Работа программы завершена с ошибкой! /* Нажмите любую клавишу для завершения работы */", 
                    Justify.Center, 
                    Constants.Colors.ErrorColor);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(status), status, null);
        }
        
        AnsiConsole.Console.Input.ReadKey(true);
        return (int)status;
    }
}