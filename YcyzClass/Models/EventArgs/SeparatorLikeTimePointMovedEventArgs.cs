using System.Windows;
using Avalonia.Interactivity;
using YcyzClass.Controls;
using YcyzClass.Controls.TimeLine;
using YcyzClass.Shared.Models.Profile;

namespace YcyzClass.Models.EventArgs;

public class SeparatorLikeTimePointMovedEventArgs(TimeLayoutItem item) : RoutedEventArgs(TimeLineListItemSeparatorAdornerControl.SeparatorLikeTimePointMovedEvent)
{
    public TimeLayoutItem Item { get; } = item;
}