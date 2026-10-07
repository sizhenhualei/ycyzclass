using System;
using Avalonia.Interactivity;
using YcyzClass.Shared.Models.Profile;

namespace YcyzClass.Controls.ScheduleDataGrid;

public class ScheduleDataGridClassPlanEventArgs(RoutedEvent e) : RoutedEventArgs(e)
{
    public required ClassPlan ClassPlan { get; init; }
    
    public required DateTime Date { get; set; }
}