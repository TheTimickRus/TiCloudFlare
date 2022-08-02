#nullable enable

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Extensions.Hosting;
using TiCloudFlareConfig.WPF.Views;
using Wpf.Ui.Mvvm.Contracts;

namespace TiCloudFlareConfig.WPF.Services
{
    public class ApplicationHostService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly INavigationService _navigationService;
        private readonly IPageService _pageService;
        private readonly IThemeService _themeService;
        private readonly ITaskBarService _taskBarService;

        private INavigationWindow? _navigationWindow;

        public ApplicationHostService(
            IServiceProvider serviceProvider,
            INavigationService navigationService,
            IPageService pageService,
            IThemeService themeService,
            ITaskBarService taskBarService)
        {
            _serviceProvider = serviceProvider;
            _navigationService = navigationService;
            _pageService = pageService;
            _themeService = themeService;
            _taskBarService = taskBarService;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            PrepareNavigation();

            await HandleActivationAsync();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
        }

        private async Task HandleActivationAsync()
        {
            await Task.CompletedTask;

            if (!Application.Current.Windows.OfType<Container>().Any())
            {
                _navigationWindow = _serviceProvider.GetService(typeof(INavigationWindow)) as INavigationWindow;
                _navigationWindow!.ShowWindow();

                // NOTICE: You can set this service directly in the window 
                // _navigationWindow.SetPageService(_pageService);

                // NOTICE: In the case of this window, we navigate to the Dashboard after loading with Container.InitializeUi()
                // _navigationWindow.Navigate(typeof(Views.Pages.Dashboard));
            }

            await Task.CompletedTask;
        }

        private void PrepareNavigation()
        {
            _navigationService.SetPageService(_pageService);
        }
    }
}