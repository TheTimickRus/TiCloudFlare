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
            WindowStyle = WindowStyle.SingleBorderWindow,
            ResizeMode = ResizeMode.CanMinimize,
            Title = caption,
            Content = message,
            ButtonLeftAppearance = ControlAppearance.Transparent,
            ButtonLeftName = "",
            ButtonRightName = "OK"
        };
        mb.ButtonRightClick += (_, _) =>
        {
            mb.Hide();
        };
    }
}