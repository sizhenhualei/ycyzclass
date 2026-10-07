using System.Collections.Generic;
using Avalonia.Interactivity;
using YcyzClass.Core.Models.Components;

namespace YcyzClass.Controls.EditMode;

public class RequestAddComponentEventArgs(RoutedEvent e) : RoutedEventArgs(e)
{
    public required IList<ComponentSettings> ComponentList { get; init; }
}