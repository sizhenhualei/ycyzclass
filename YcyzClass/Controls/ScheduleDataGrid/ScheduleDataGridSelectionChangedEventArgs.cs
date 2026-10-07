using System;
using Avalonia.Interactivity;
using YcyzClass.Shared.Models.Profile;

namespace YcyzClass.Controls.ScheduleDataGrid;

public class ScheduleDataGridSelectionChangedEventArgs(RoutedEvent e) : RoutedEventArgs(e)
{
    public required ClassInfo ClassInfo { get; init; }
    
    public required DateTime Date { get; init; }
}