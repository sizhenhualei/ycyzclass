using System.Globalization;

using Avalonia.Data.Converters;
using YcyzClass.Shared;
using YcyzClass.Shared.ComponentModels;
using YcyzClass.Shared.Interfaces;

namespace YcyzClass.Core.Converters;

public class MiniInfoGuidToMiniInfoProviderElementMultiConverter : IMultiValueConverter
{
    public object? Convert(IList<object?> values, Type targetType, object parameter, CultureInfo culture)
    {
        var guid = (string?)values[0];
        if (guid == null) return null;
        var providers = (ObservableDictionary<string, IMiniInfoProvider>)values[1];
        if (!providers.ContainsKey(guid)) return null;
        return providers[guid].InfoElement;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        return Array.Empty<object>();
    }
}