using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TiCloudFlareConfig.WPF.Models.Pages.Configs;
using TiCloudFlareConfig.WPF.Services.Database;

namespace TiCloudFlareConfig.WPF.ViewModels.Pages;

[ObservableObject]
public partial class ConfigsPageViewModel
{
    [ObservableProperty]
    private List<Config> _configs;

    private readonly IDataBaseService _dataBaseService;
    
    public ConfigsPageViewModel(IDataBaseService dataBaseService)
    {
        _dataBaseService = dataBaseService;
        _configs = _dataBaseService.FetchAllConfigs();
    }

    [RelayCommand]
    private void Update()
    {
        _configs = _dataBaseService.FetchAllConfigs();
    }
}