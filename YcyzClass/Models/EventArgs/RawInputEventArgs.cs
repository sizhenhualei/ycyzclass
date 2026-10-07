using Linearstar.Windows.RawInput;

namespace YcyzClass.Models.EventArgs;

public class RawInputEventArgs(RawInputData data) : System.EventArgs
{
    public RawInputData Data { get; } = data;
}