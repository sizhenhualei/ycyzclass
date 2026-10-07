using CommunityToolkit.Mvvm.ComponentModel;

namespace YcyzClass.ViewModels.SettingsPages;

public partial class ErrorSettingsViewModel : ObservableRecipient
{
    [ObservableProperty] private bool _isError = false;
}