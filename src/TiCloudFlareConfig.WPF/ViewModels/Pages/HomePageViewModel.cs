#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TiCloudFlareConfig.Shared.LicGenerate;
using TiCloudFlareConfig.Shared.WireGuardConfig;
using TiCloudFlareConfig.Shared.WireGuardConfig.Models;
using TiCloudFlareConfig.WPF.Models.Pages.Configs;
using TiCloudFlareConfig.WPF.Services.Database;
using TiCloudFlareConfig.WPF.Services.TiDialog;
using Wpf.Ui.Controls;

namespace TiCloudFlareConfig.WPF.ViewModels.Pages;

public partial class HomePageViewModel : ObservableObject
{
    [ObservableProperty]
    private string? _licenseKey;
    
    [ObservableProperty]
    private List<string> _endPoints = new();
    [ObservableProperty]
    private List<string> _endPointPorts = new();
    [ObservableProperty]
    private List<string> _mtu = new();
    
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
        
        _keysResponse = WireGuardConfig.FetchKeys();
        
        EndPoints = _keysResponse.EndPoints;
        EndPointPorts = _keysResponse.Ports;
        Mtu = _keysResponse.MTU;
        SelectedEndPointIndex = 0;
        SelectedEndPointPortIndex = 0;
        SelectedMtuIndex = 0;
    }

    [RelayCommand]
    private void GenerateLic()
    {
        TiDialog.QuestionDialog("Вы действительно этого хотите?\nНа это действие действует лимитное ограничение...",
            async (sender, _) =>
            {
                (sender as Dialog)?.Hide();
                
                var dialog = TiDialog.ProgressDialog();
        
                var accountInfo = await new GenerateLicenseService(_keysResponse.WarpKeys).GenerateAsync();
                Guard.Against.Null(accountInfo);

                LicenseKey = accountInfo.License;
        
                TiDialog.Hide(dialog);
            });
    }
    
    [RelayCommand]
    private void Reset()
    {
        TiDialog.QuestionDialog("Вы действительно этого хотите?", (sender, _) =>
        {
            (sender as Dialog)?.Hide();
            
            LicenseKey = "";
            SelectedEndPointIndex = 0;
            SelectedEndPointPortIndex = 0;
            SelectedMtuIndex = 0;
        });
    }
    
    [RelayCommand]
    private async Task GenerateConfig()
    {
        var dialog = TiDialog.ProgressDialog();
        
        try
        {
            var configParams = new WireGuardConfigParams
            {
                License = LicenseKey,
                EndPoint = SelectedEndPoint,
                Port = SelectedEndPointPort,
                Mtu = SelectedMtu
            };

            var configResponse = await WireGuardConfig.RegisterAsync(configParams);
            Guard.Against.Null(configResponse.FileConfig);
            Guard.Against.Null(configResponse.FileToml);
        
            _dataBaseService.AddConfig(new ConfigItem
            {
                Title = Path.GetFileNameWithoutExtension(configResponse.FileConfig),
                CreationAt = DateTime.Now,
                IsWarpPlus = configParams.IsLicGenerate,
                FileConfig = await File.ReadAllTextAsync(configResponse.FileConfig),
                FileToml = await File.ReadAllTextAsync(configResponse.FileToml)
            });
            
            WireGuardConfig.DeleteTempFiles();
        }
        catch (Exception ex)
        {
            TiDialog.ErrorDialog(ex.Message);
        }
        
        TiDialog.Hide(dialog);
    }
}