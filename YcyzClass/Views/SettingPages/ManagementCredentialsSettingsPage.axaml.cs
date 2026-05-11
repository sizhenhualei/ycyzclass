using System.Windows;
using Avalonia.Interactivity;
using YcyzClass.Core.Abstractions.Controls;
using YcyzClass.Core.Abstractions.Services.Management;
using YcyzClass.Core.Attributes;
using YcyzClass.Core.Enums.SettingsWindow;
using YcyzClass.ViewModels.SettingsPages;

namespace YcyzClass.Views.SettingPages;

/// <summary>
/// ManagementCredentialsSettingsPage.xaml 的交互逻辑
/// </summary>
[SettingsPageInfo("management.credentials", "集控凭据设置", true, SettingsPageCategory.About)]
public partial class ManagementCredentialsSettingsPage : SettingsPageBase
{
    public ManagementCredentialsSettingsViewModel ViewModel { get; } = new();

    public IManagementService ManagementService { get; }

    public ManagementCredentialsSettingsPage(IManagementService managementService)
    {
        ManagementService = managementService;
        DataContext = this;
        InitializeComponent();
    }

    private async void ManagementCredentialsSettingsPage_OnLoaded(object sender, RoutedEventArgs e)
    {
        if (ManagementService.IsManagementEnabled)
        {
            return;
        }
        var result =
            await ManagementService.AuthorizeByLevel(ManagementService.CredentialConfig
                .EditAuthorizeSettingsAuthorizeLevel);
        if (result)
        {
            ViewModel.IsLocked = false;
        }
    }

    private void ManagementCredentialsSettingsPage_OnUnloaded(object sender, RoutedEventArgs e)
    {
        ViewModel.IsLocked = true;
    }
}
