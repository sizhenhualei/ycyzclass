using System;
using YcyzClass.Core.Abstractions.Services;
using YcyzClass.Shared;
namespace YcyzClass.Converters;

public static class DateTimeToCurrentDateTimeConverter
{
    public static DateTime Convert(DateTime dateTime)
    {
        var now = IAppHost.GetService<IExactTimeService>().GetCurrentLocalDateTime();
        return new DateTime(now.Year, now.Month, now.Day, dateTime.Hour, dateTime.Minute,
            dateTime.Second);
    }
}