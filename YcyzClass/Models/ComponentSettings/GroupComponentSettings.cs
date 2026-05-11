using System.Collections.ObjectModel;
using YcyzClass.Core.Abstractions.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace YcyzClass.Models.ComponentSettings;

public partial class GroupComponentSettings : ObservableObject, IComponentContainerSettings
{
    [ObservableProperty]
    private ObservableCollection<Core.Models.Components.ComponentSettings> _children = [];
}