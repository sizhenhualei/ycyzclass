using Avalonia.Controls;
using Avalonia.Data.Converters;

namespace YcyzClass.Core.Converters;

public class ElementConverters
{
    public static readonly FuncValueConverter<object?, object> ControlPreventNullConverter = new(x =>
    {
        return x ?? new Border();
    });
}
