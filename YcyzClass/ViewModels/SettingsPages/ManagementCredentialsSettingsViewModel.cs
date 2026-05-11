using CommunityToolkit.Mvvm.ComponentModel;

namespace YcyzClass.ViewModels.SettingsPages;

public partial class ManagementCredentialsSettingsViewModel : ObservableObject
{
    [ObservableProperty] private bool _isLocked = true;
}