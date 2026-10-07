using System.Globalization;
using Avalonia.Data.Converters;
using YcyzClass.Shared;
using YcyzClass.Shared.ComponentModels;
using YcyzClass.Shared.Models.Profile;

namespace YcyzClass.Core.Converters;

public class SubjectsDictionaryValueAccessConverter : IValueConverter
{
    public ObservableDictionary<string, Subject> SourceDictionary
    {
        get;
        set;
    } = new();

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var k = (string)value;
        try
        {
            return SourceDictionary[k].Name;
        }
        catch
        {
            return k;
        }
    }

    public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
}