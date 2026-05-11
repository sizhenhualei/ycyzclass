using CommunityToolkit.Mvvm.ComponentModel;

namespace YcyzClass.ViewModels;

public partial class RecoveryViewModel : ObservableObject
{
    [ObservableProperty] private bool _canGoBack;
}