using Avalonia.Interactivity;
using YcyzClass.Shared.Models.Profile;

namespace YcyzClass.Controls.TimeLine;

public class TimeLineInsertTimePointEventArgs(RoutedEvent e) : RoutedEventArgs(e)
{
    public required int Kind { get; init; }
    
    public required InsertLocation Location { get; init; }
    
    public required TimeLayoutItem TimePoint { get; init; }
    
    public enum InsertLocation
    {
        Before,
        After,
        Inside
    }
}