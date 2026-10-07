using System;

namespace YcyzClass.Controls.ScheduleDataGrid;

public class CreateClassPlanEventArgs : EventArgs
{
    public required DateTime Date { get; init; }
}