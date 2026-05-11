using Avalonia;

namespace YcyzClass.Models.EventArgs;

public class MousePosChangedEventArgs(Point pos) : System.EventArgs
{
    public Point Pos { get; } = pos;
}