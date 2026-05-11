using System;
using System.Linq;
using YcyzClass.Services;
using YcyzClass.Shared.Helpers;
using YcyzClass.Shared.Models.Profile;

namespace YcyzClass.Helpers.ProfileTransferHelpers;

public static class YcyzClassV1ProfileTransferHelper
{
    [Obsolete]
    public static Profile TransferYcyzClassV1ProfileToYcyzClassProfile(string path)
    {
        var config = ConfigureFileHelper.LoadConfigUnWrapped<Profile>(path, false);
        foreach (var tl in config.TimeLayouts)
        {
            foreach (var layoutItem in tl.Value.Layouts.Where(x =>
                         !string.IsNullOrWhiteSpace(x.StartSecond) && !string.IsNullOrWhiteSpace(x.EndSecond)))
            {
                layoutItem.StartTime = DateTime.TryParse(layoutItem.StartSecond, out var r1)
                    ? r1.TimeOfDay
                    : TimeSpan.Zero;
                layoutItem.EndTime = DateTime.TryParse(layoutItem.EndSecond, out var r2)
                    ? r2.TimeOfDay
                    : TimeSpan.Zero;
            }
        }

        return config;
    }
}