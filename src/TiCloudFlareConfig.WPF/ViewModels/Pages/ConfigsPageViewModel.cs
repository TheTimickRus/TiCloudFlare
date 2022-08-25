#nullable enable

using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using Ardalis.GuardClauses;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ICSharpCode.SharpZipLib.Zip;
using Microsoft.Win32;
using TiCloudFlareConfig.WPF.Models.Pages.Configs;
using TiCloudFlareConfig.WPF.Services.Database;
using TiCloudFlareConfig.WPF.Services.TiDialog;
using Wpf.Ui.Controls;

namespace TiCloudFlareConfig.WPF.ViewModels.Pages;

[ObservableObject]
public partial class ConfigsPageViewModel
{
    #region Properties

    [ObservableProperty]
    private ObservableCollection<ConfigItem> _configs;

    [ObservableProperty]
    private ConfigItem? _selectedConfigItem;

    [ObservableProperty]
    private Visibility _placeholderVisibility;
    
    #endregion
    
    private readonly IDataBaseService _dataBaseService;
    
    public ConfigsPageViewModel(IDataBaseService dataBaseService)
    {
        _dataBaseService = dataBaseService;
        _configs = new ObservableCollection<ConfigItem>(_dataBaseService.FetchAllConfigs());
        _placeholderVisibility = _configs.Count > 0 ? Visibility.Hidden : Visibility.Visible;
    }

    #region Commands

    [RelayCommand]
    private void Export(object? type)
    {
        var dialog = TiDialog.ProgressDialog();
        
        try
        {
            Guard.Against.Null(SelectedConfigItem);
        
            switch (type)
            {
                case "config":
                    SaveConfigs(SelectedConfigItem);
                    break;
                case "zip":
                    SaveZip(SelectedConfigItem);
                    break;
            }
        }
        catch (Exception ex)
        {
            TiDialog.ErrorDialog(ex.Message);
        }
        
        TiDialog.Hide(dialog);
    }

    [RelayCommand]
    private void Update()
    {
        try
        {
            Configs = new ObservableCollection<ConfigItem>(_dataBaseService.FetchAllConfigs());
            PlaceholderVisibility = Configs.Count > 0 ? Visibility.Hidden : Visibility.Visible;
        }
        catch (Exception ex)
        {
            TiDialog.ErrorDialog(ex.StackTrace);
        }
    }
    
    [RelayCommand]
    private void Delete()
    {
        try
        {
            TiDialog.QuestionDialog("Вы действительно этого хотите?", (sender, _) =>
            {
                (sender as Dialog)?.Hide();

                Guard.Against.Null(SelectedConfigItem);
                
                _dataBaseService.RemoveConfig(SelectedConfigItem.Id);
                Configs.Remove(SelectedConfigItem);
                
                PlaceholderVisibility = Configs.Count > 0 ? Visibility.Hidden : Visibility.Visible;
            });
        }
        catch (Exception ex)
        {
            TiDialog.ErrorDialog(ex.Message);
        }
    }

    [RelayCommand]
    private void DeleteAll()
    {
        try
        {
            TiDialog.QuestionDialog("Вы действительно этого хотите?", (sender, _) =>
            {
                (sender as Dialog)?.Hide();
                
                _dataBaseService.RemoveAllConfigs();
                Configs.Clear();
                
                PlaceholderVisibility = Configs.Count > 0 ? Visibility.Hidden : Visibility.Visible;
            });
        }
        catch (Exception ex)
        {
            TiDialog.ErrorDialog(ex.Message);
        }
    }

    #endregion

    #region Private Methods

    private static void SaveConfigs(ConfigItem item)
    {
        var configFileName = SaveDialog("*.conf|*.conf");
        Guard.Against.Null(configFileName);
        var tomlFileName = Path.Combine(
            $"{Path.GetDirectoryName(configFileName)}", 
            $"{Path.GetFileNameWithoutExtension(configFileName)}.toml");
        Guard.Against.Null(tomlFileName);
        
        File.WriteAllText(configFileName, item.FileConfig);
        File.WriteAllText(tomlFileName, item.FileToml);
    }

    private static void SaveZip(ConfigItem item)
    {
        var zipFileName = SaveDialog("*.zip|*.zip");
        Guard.Against.Null(zipFileName);
        
        var configFileName = $"{Path.GetFileNameWithoutExtension(zipFileName)}.conf";
        var tomlFileName = $"{Path.GetFileNameWithoutExtension(zipFileName)}.toml";
        
        File.WriteAllText(configFileName, item.FileConfig);
        File.WriteAllText(tomlFileName, item.FileToml);
        
        using (var zipFile = ZipFile.Create(zipFileName))
        {
            zipFile.BeginUpdate();
            zipFile.Add(configFileName);
            zipFile.Add(tomlFileName);
            zipFile.CommitUpdate();
            zipFile.Close();
        }
        
        File.Delete(configFileName);
        File.Delete(tomlFileName);
    }
    
    private static string? SaveDialog(string filter)
    {
        var dialog = new SaveFileDialog
        {
            Title = "Пожалуйста, выберите, куда сохранять файл...",
            Filter = filter,
            FileName = "TiCloudFlare",
            InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
        };
        
        var isShow = dialog.ShowDialog() ?? false;
        
        return isShow ? dialog.FileName : null;
    }

    #endregion
}