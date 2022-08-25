using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Wpf.Ui.Appearance;
using Wpf.Ui.Common;
using Wpf.Ui.Controls.Interfaces;
using Wpf.Ui.Mvvm.Contracts;
using Wpf.Ui.TaskBar;

namespace TiCloudFlareConfig.WPF.Views
{
    public partial class Container : INavigationWindow
    {
        private bool _initialized;

        private readonly ITaskBarService _taskBarService;

        public Container(
            INavigationService navigationService,
            IPageService pageService,
            ITaskBarService taskBarService)
        {
            _taskBarService = taskBarService;

            InitializeComponent();

            SetPageService(pageService);
            navigationService.SetNavigationControl(RootNavigation);

            Loaded += (_, _) => InvokeSplashScreen();

            Watcher.Watch(this);
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Application.Current.Shutdown();
        }

        #region INavigationWindow methods

        public Frame GetFrame()
            => RootFrame;

        public INavigation GetNavigation()
            => RootNavigation;

        public bool Navigate(Type pageType)
            => RootNavigation.Navigate(pageType);

        public void SetPageService(IPageService pageService)
            => RootNavigation.PageService = pageService;

        public void ShowWindow()
            => Show();

        public void CloseWindow()
            => Close();

        #endregion INavigationWindow methods

        private void InvokeSplashScreen()
        {
            if (_initialized)
                return;

            _initialized = true;

            RootMainGrid.Visibility = Visibility.Collapsed;
            RootWelcomeGrid.Visibility = Visibility.Visible;

            _taskBarService.SetState(this, TaskBarProgressState.Indeterminate);

            Task.Run(async () =>
            {
                await Task.Delay(2000);

                await Dispatcher.InvokeAsync(() =>
                {
                    RootWelcomeGrid.Visibility = Visibility.Hidden;
                    RootMainGrid.Visibility = Visibility.Visible;

                    Navigate(typeof(Pages.HomePage));

                    _taskBarService.SetState(this, TaskBarProgressState.None);
                });

                return true;
            });
        }

        private void RootNavigation_OnNavigated(INavigation sender, RoutedNavigationEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"DEBUG | WPF UI Navigated to: {sender.Current ?? null}", "Wpf.Ui.Demo");

            RootFrame.Margin = new Thickness(
                left: 0,
                top: sender.Current?.PageTag == "FirstPage" ? -69 : 0,
                right: 0,
                bottom: 0);
        }
    }
}