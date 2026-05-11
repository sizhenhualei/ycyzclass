using CommunityToolkit.Mvvm.ComponentModel;

namespace YcyzClass.Models.Actions;

public partial class WeatherNotificationActionSettings : ObservableObject
{
    [ObservableProperty] private int _notificationKind = 0;
}