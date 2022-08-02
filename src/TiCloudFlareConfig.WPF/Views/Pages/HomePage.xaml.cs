using TiCloudFlareConfig.WPF.ViewModels.Pages;

namespace TiCloudFlareConfig.WPF.Views.Pages
{
    public partial class HomePage
    {
        private readonly HomePageViewModel _viewModel;
        
        public HomePage(HomePageViewModel viewModel)
        {
            DataContext = viewModel;
            _viewModel = viewModel;
            
            InitializeComponent();
        }
    }
}