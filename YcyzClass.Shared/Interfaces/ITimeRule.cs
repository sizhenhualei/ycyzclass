using System.ComponentModel;

namespace YcyzClass.Shared.Interfaces;

public interface ITimeRule : INotifyPropertyChanged
{
    public string Name
    {
        get;
        set;
    }

    public bool IsSatisfied
    {
        get; set;
    }
}