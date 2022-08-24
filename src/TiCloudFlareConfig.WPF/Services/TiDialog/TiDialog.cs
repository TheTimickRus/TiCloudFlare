#nullable enable

using System.Windows;
using Ardalis.GuardClauses;
using TiCloudFlareConfig.WPF.Views;
using Wpf.Ui.Controls;

namespace TiCloudFlareConfig.WPF.Services.TiDialog;

public static class TiDialog
{
    public static Dialog ProgressDialog()
    {
        var dialog = (Application.Current.MainWindow as Container)?.PDialog;
        
        Guard.Against.Null(dialog);
        
        dialog.Show();
        return dialog;
    }

    public static void QuestionDialog(string message, RoutedEventHandler buttonLeftClick)
    {
        var dialog = (Application.Current.MainWindow as Container)?.QDialog;
        
        Guard.Against.Null(dialog);

        dialog.Title = "QDialog";
        dialog.Message = message;
        dialog.ButtonLeftClick += buttonLeftClick;
        dialog.ButtonRightClick += (_, _) => { dialog.Hide(); };
        dialog.Show();
    }
    
    public static void ErrorDialog(string? message)
    {
        var dialog = (Application.Current.MainWindow as Container)?.EDialog;

        Guard.Against.Null(dialog);
        
        dialog.Title = "EDialog";
        dialog.Message = message ?? "";
        dialog.ButtonRightClick += (_, _) => { dialog.Hide(); };
        dialog.Show();
    }
    
    public static void Hide(Dialog dialog)
    {
        dialog.Hide();
    }
}