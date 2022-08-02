using TiCloudFlareConfig.WPF.ViewModels.Pages;

namespace TiCloudFlareConfig.WPF.Views.Pages
{
    public partial class SettingsPage
    {
        public SettingsPage(SettingsPageViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}