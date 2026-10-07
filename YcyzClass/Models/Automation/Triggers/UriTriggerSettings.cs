using CommunityToolkit.Mvvm.ComponentModel;

namespace YcyzClass.Models.Automation.Triggers;

public partial class UriTriggerSettings : ObservableObject
{
    [ObservableProperty] private string _uriSuffix = "";
}