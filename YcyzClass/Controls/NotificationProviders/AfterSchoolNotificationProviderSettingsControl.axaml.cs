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
/// AfterSchoolNotificationProviderSettingsControl.xaml 的交互逻辑
/// </summary>
public partial class AfterSchoolNotificationProviderSettingsControl : NotificationProviderControlBase<AfterSchoolNotificationProviderSettings>
{
    public AfterSchoolNotificationProviderSettingsControl()
    {
        InitializeComponent();
    }

    private void ButtonShowAttachedSettingsInfo_OnClick(object sender, RoutedEventArgs e)
    {
        SettingsPageBase.OpenDrawerCommand.Execute(new RootAttachedSettingsDependencyControl(IAttachedSettingsHostService.RegisteredControls.First(x => x.Guid == new Guid("8FBC3A26-6D20-44DD-B895-B9411E3DDC51"))));
    }
}
