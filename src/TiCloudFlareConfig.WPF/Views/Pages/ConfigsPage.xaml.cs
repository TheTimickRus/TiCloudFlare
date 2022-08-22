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
}