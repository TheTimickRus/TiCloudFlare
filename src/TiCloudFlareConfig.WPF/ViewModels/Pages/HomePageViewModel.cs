#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using Ardalis.GuardClauses;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using TiCloudFlareConfig.Shared.LicGenerate;
using TiCloudFlareConfig.Shared.WireGuardConfig;
using TiCloudFlareConfig.Shared.WireGuardConfig.Models;
using TiCloudFlareConfig.WPF.Models.Pages.Configs;
using TiCloudFlareConfig.WPF.Services.Database;
using TiCloudFlareConfig.WPF.Services.TiMessageBox;
using TiCloudFlareConfig.WPF.Views;

namespace TiCloudFlareConfig.WPF.ViewModels.Pages;

[ObservableObject]
public partial class HomePageViewModel
{
    [ObservableProperty]
    private string? _licenseKey;
    
    [ObservableProperty]
    private List<string> _endPoints;
    [ObservableProperty]
    private List<string> _endPointPorts;
    [ObservableProperty]
    private List<string> _mtu;
    
    [ObservableProperty]
    private string? _selectedEndPoint;
    [ObservableProperty]
    private int _selectedEndPointIndex;
    [ObservableProperty]
    private string? _selectedEndPointPort;
    [ObservableProperty]
    private int _selectedEndPointPortIndex;
    [ObservableProperty]
    private string? _selectedMtu;
    [ObservableProperty]
    private int _selectedMtuIndex;
    
    private readonly KeysResponse _keysResponse;
    
    private readonly IDataBaseService _dataBaseService;
    
    public HomePageViewModel(IDataBaseService dataBaseService)
    {
        _dataBaseService = dataBaseService;
        
        WireGuardConfig.ExtractResources();
        _keysResponse = WireGuardConfig.FetchKeys();
        
        _endPoints = _keysResponse.EndPoints;
        _endPointPorts = _keysResponse.Ports;
        _mtu = _keysResponse.MTU;
        _selectedEndPointIndex = 0;
        _selectedEndPointPortIndex = 0;
        _selectedMtuIndex = 0;
    }

    [RelayCommand]
    private async Task GenerateLic()
    {
        var dialog = (Application.Current.MainWindow as Container)?.PDialog;
        dialog?.Show();

        var accountInfo = await new GenerateLicenseService(_keysResponse.WarpKeys).GenerateAsync();
        Guard.Against.Null(accountInfo);

        LicenseKey = accountInfo.License;
        
        dialog?.Hide();
    }
    
    [RelayCommand]
    private void Reset()
    {
        var dialog = (Application.Current.MainWindow as Container)?.QDialog;
        
        Guard.Against.Null(dialog);
        
        dialog.Show();
        dialog.ButtonLeftClick += (_, _) =>
        {
            LicenseKey = "";
            SelectedEndPointIndex = 0;
            SelectedEndPointPortIndex = 0;
            SelectedMtuIndex = 0;
            
            dialog.Hide();
        };
        dialog.ButtonRightClick += (_, _) =>
        {
            dialog.Hide();
        };
    }
    
    [RelayCommand]
    private async Task GenerateConfig()
    {
        var dialog = (Application.Current.MainWindow as Container)?.PDialog;
        dialog?.Show();
        
        try
        {
            var configParams = new WireGuardConfigParams
            {
                License = _licenseKey,
                EndPoint = _selectedEndPoint,
                Port = _selectedEndPointPort,
                Mtu = _selectedMtu
            };

            var configResponse = await WireGuardConfig.RegisterAsync(configParams);
            Guard.Against.Null(configResponse.FileConfig);
            Guard.Against.Null(configResponse.FileToml);
        
            _dataBaseService.AddConfig(new Config
            {
                Title = Path.GetFileNameWithoutExtension(configResponse.FileConfig),
                CreationAt = DateTime.Now,
                IsWarpPlus = configParams.IsLicGenerate,
                FileConfig = await File.ReadAllTextAsync(configResponse.FileConfig),
                FileToml = await File.ReadAllTextAsync(configResponse.FileToml)
            });
        }
        catch(Exception ex)
        {
            TiMessageBox.ShowError(ex.Message, "Error!");
        }
        
        dialog?.Hide();
    }
}