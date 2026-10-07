using CommunityToolkit.Mvvm.ComponentModel;

namespace YcyzClass.ViewModels.SettingsPages;

public partial class ManagementPolicySettingsViewModel : ObservableObject
{
    [ObservableProperty] private bool _isLocked = true;
}