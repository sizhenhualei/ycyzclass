#if false
using System.Windows;
using YcyzClass.Core;
using YcyzClass.Shared;
using YcyzClass.Shared.Models;
using CommunityToolkit.Mvvm.Input;

namespace YcyzClass.Views;

/// <summary>
/// ConfigErrorsWindow.xaml 的交互逻辑
/// </summary>
public partial class ConfigErrorsWindow
{
    public ConfigErrorsWindow()
    {
        InitializeComponent();
        DataContext = this;
    }

    [RelayCommand]
    private void CopyErrorDetails(ConfigError error)
    {
        Clipboard.SetDataObject(error.Exception.ToString(), false);
    }

    private void ButtonRestoreBackups_OnClick(object sender, RoutedEventArgs e)
    {
        AppBase.Current.Restart(["-m", "-r"]);
    }
}
#endif
