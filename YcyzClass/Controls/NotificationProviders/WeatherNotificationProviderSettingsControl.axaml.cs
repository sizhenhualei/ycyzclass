using System;
using System.Linq;
using System.Windows;
using Avalonia.Interactivity;
using YcyzClass.Core.Abstractions.Controls;
using YcyzClass.Core.Abstractions.Services;
using YcyzClass.Core.Controls;
using YcyzClass.Models.NotificationProviderSettings;

namespace YcyzClass.Controls.NotificationProviders;

/// <summary>
/// WeatherNotificationProviderSettingsControl.xaml 的交互逻辑
/// </summary>
public partial class WeatherNotificationProviderSettingsControl : NotificationProviderControlBase<
    WeatherNotificationProviderSettings>
{

    public WeatherNotificationProviderSettingsControl()
    {
        InitializeComponent();
    }

    private void ButtonShowAttachedSettingsInfo_OnClick(object sender, RoutedEventArgs e)
    {
        SettingsPageBase.OpenDrawerCommand.Execute(new RootAttachedSettingsDependencyControl(IAttachedSettingsHostService.RegisteredControls.First(x => x.Guid == new Guid("7625DE96-38AA-4B71-B478-3F156DD9458D"))));
    }
}
