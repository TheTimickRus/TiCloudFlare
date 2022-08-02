#nullable enable

using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TiCloudFlareConfig.WPF.Models;
using TiCloudFlareConfig.WPF.Services;
using TiCloudFlareConfig.WPF.ViewModels.Pages;
using TiCloudFlareConfig.WPF.Views;
using TiCloudFlareConfig.WPF.Views.Pages;
using Wpf.Ui.Common;
using Wpf.Ui.Controls;
using Wpf.Ui.Mvvm.Contracts;
using Wpf.Ui.Mvvm.Services;
using MessageBox = Wpf.Ui.Controls.MessageBox;

namespace TiCloudFlareConfig.WPF.App
{
    public partial class App
    {
        private IHost? _host;

        private async void OnStartup(object sender, StartupEventArgs e)
        {
            _host = Host.CreateDefaultBuilder(e.Args)
                .ConfigureAppConfiguration(c =>
                {
                    c.SetBasePath(Path.GetDirectoryName(System.AppContext.BaseDirectory)!);
                })
                .ConfigureServices(ConfigureServices)
                .Build();

            await _host.StartAsync();
        }

        private static void ConfigureServices(HostBuilderContext context, IServiceCollection services)
        {
            services.AddHostedService<ApplicationHostService>();

            services.AddSingleton<IThemeService, ThemeService>();
            services.AddSingleton<ITaskBarService, TaskBarService>();
            services.AddSingleton<IPageService, PageService>();
            services.AddSingleton<INavigationService, NavigationService>();

            services.AddScoped<INavigationWindow, Container>();

            services.AddScoped<HomePage>();
            services.AddScoped<HomePageViewModel>();

            services.AddScoped<SettingsPage>();
            services.AddScoped<SettingsPageViewModel>();

            services.Configure<AppConfig>(context.Configuration.GetSection(nameof(AppConfig)));
        }

        private async void OnExit(object sender, ExitEventArgs e)
        {
            if (_host != null)
            {
                await _host.StopAsync();
                _host.Dispose();
            }

            _host = null;
        }

        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            
            var mb = new MessageBox
            {
                WindowStyle = WindowStyle.SingleBorderWindow,
                ResizeMode = ResizeMode.CanMinimize,
                Title = "Необработанное исключение!",
                Content = e.Exception.Message,
                ButtonLeftName = "OK",
                ButtonLeftAppearance = ControlAppearance.Success,
                ButtonRightName = "Закрыть приложение",
                ButtonRightAppearance = ControlAppearance.Danger
            };
            
            mb.ButtonLeftClick += (_, _) => mb.Close();
            mb.ButtonRightClick += (_, _) => Current.Shutdown();
        }
    }
}