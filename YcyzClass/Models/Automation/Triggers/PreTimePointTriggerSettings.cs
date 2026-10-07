using YcyzClass.Core.Attributes;
using YcyzClass.Shared.Enums;
using CommunityToolkit.Mvvm.ComponentModel;

namespace YcyzClass.Models.Automation.Triggers;

public partial class PreTimePointTriggerSettings : ObservableObject
{
    [ObservableProperty] private TimeState _targetState = TimeState.OnClass;

    [ObservableProperty] private double _timeSeconds = 60;
}