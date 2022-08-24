using System.Windows;
using TiCloudFlareConfig.WPF.ViewModels.Pages;

namespace TiCloudFlareConfig.WPF.Views.Pages;

public partial class ConfigsPage
{
    private readonly ConfigsPageViewModel _viewModel;
    
    public ConfigsPage(ConfigsPageViewModel viewModel)
    {
        DataContext = viewModel;
        _viewModel = viewModel;
        
        InitializeComponent();
    }

    private void ConfigsPage_OnLoaded(object sender, RoutedEventArgs e)
    {
        _viewModel.UpdateCommand.Execute(null);
    }
}