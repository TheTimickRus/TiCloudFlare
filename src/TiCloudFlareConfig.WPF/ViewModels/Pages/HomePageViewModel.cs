#nullable enable

using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using Ardalis.GuardClauses;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Ookii.Dialogs.Wpf;
using TiCloudFlareConfig.Shared.LicGenerate;
using TiCloudFlareConfig.Shared.WireGuardConfig;
using TiCloudFlareConfig.Shared.WireGuardConfig.Models;
using TiCloudFlareConfig.WPF.Views;

namespace TiCloudFlareConfig.WPF.ViewModels.Pages;

public class HomePageViewModel : ObservableObject
{
    public string? LicenseKey { get; set; }
    
    public ObservableCollection<string> EndPoints { get; } = new()
    {
        "engage.cloudflareclient.com",
        "162.159.193.1",
        "162.159.193.2",
        "162.159.193.3",
        "162.159.193.4",
        "162.159.193.5",
        "162.159.193.6",
        "162.159.193.7",
        "162.159.193.8",
        "162.159.193.9",
    };
    public ObservableCollection<string> EndPointPorts { get; } = new()
    {
        "2408",
        "500",
        "1701",
        "4500"
    };
    public ObservableCollection<string> Mtu { get; } = new()
    {
        "1280",
        "1500",
        "1492",
        "1440",
        "1420",
        "1392"
    };
    public string? SelectedEndPoint { get; set; }
    public int SelectedEndPointIndex {get; set; }
    public string? SelectedEndPointPort { get; set; }
    public int SelectedEndPointPortIndex {get; set; }
    public string? SelectedMtu { get; set; }
    public int SelectedMtuIndex { get; set; }

    public IAsyncRelayCommand BGenerateLicCommand => new AsyncRelayCommand(GenerateLic);
    public IRelayCommand BResetCommand => new RelayCommand(Reset);
    public IAsyncRelayCommand BGenerateConfigCommand => new AsyncRelayCommand(GenerateConfig);

    public HomePageViewModel()
    {
        SelectedEndPointIndex = 0;
        SelectedEndPointPortIndex = 0;
        SelectedMtuIndex = 0;
    }

    private async Task GenerateLic()
    {
        var dialog = (Application.Current.MainWindow as Container)?.IndeterminateProgressDialog;
        dialog?.Show();

        var accountInfo = await new GenerateLicenseService().GenerateAsync();
        Guard.Against.Null(accountInfo);

        LicenseKey = accountInfo.License;
        
        dialog?.Hide();
    }
    
    private void Reset()
    {
        LicenseKey = "";
        
        SelectedEndPointIndex = 0;
        SelectedEndPointPortIndex = 0;
        SelectedMtuIndex = 0;
    }
    
    private async Task GenerateConfig()
    {
        var dialog = (Application.Current.MainWindow as Container)?.IndeterminateProgressDialog;
        dialog?.Show();

        var configParams = new WireGuardConfigParams
        {
            License = LicenseKey,
            EndPoint = SelectedEndPoint,
            EndPointPort = SelectedEndPointPort,
            Mtu = SelectedMtu
        };

        var configResponse = await WireGuardConfig.RegisterAsync(configParams);
        SaveConfig(configResponse);
        
        dialog?.Hide();
    }

    private static void SaveConfig(WireGuardConfigResponse? configs)
    {
        Guard.Against.Null(configs);
        
        var dialog = new VistaFolderBrowserDialog
        {
            Description = "Выберите путь сохранения файлов конфигурации...",
            Multiselect = false,
            ShowNewFolderButton = true,
            UseDescriptionForTitle = true
        };

        if (!(dialog.ShowDialog() ?? false))
            return;
        
        WireGuardConfig.CreateArchive(
            configs, 
            Path.Combine(dialog.SelectedPath, $"{Path.GetFileName(configs.FileConfig)}.zip"));
    }
}