using System.Windows;
using Wpf.Ui.Common;
using MessageBox = Wpf.Ui.Controls.MessageBox;

namespace TiCloudFlareConfig.WPF.Services.TiMessageBox;

public static class TiMessageBox
{
    public static void ShowError(string message, string caption)
    {
        var mb = new MessageBox
        {
            Owner = Application.Current.MainWindow,
            WindowStyle = WindowStyle.SingleBorderWindow,
            ResizeMode = ResizeMode.NoResize,
            Title = caption,
            Content = message,
            ButtonLeftAppearance = ControlAppearance.Danger,
            ButtonLeftName = "Завершить работу программы",
            ButtonRightAppearance = ControlAppearance.Success,
            ButtonRightName = "Игнорировать ошибку"
        };

        mb.ButtonLeftClick += (_, _) =>
        {
            mb.Hide();
            Application.Current.Shutdown();
        };
        
        mb.ButtonRightClick += (_, _) =>
        {
            mb.Hide();
        };
        
        mb.Show();
    }
}