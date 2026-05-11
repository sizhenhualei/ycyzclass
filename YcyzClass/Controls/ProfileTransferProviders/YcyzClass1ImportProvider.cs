using System;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using YcyzClass.Core.Abstractions.Controls.ProfileTransferProviders;
using YcyzClass.Core.Abstractions.Services;
using YcyzClass.Core.Helpers.UI;
using YcyzClass.Helpers.ProfileTransferHelpers;
using YcyzClass.Services;
using YcyzClass.Shared;
using YcyzClass.Shared.Helpers;
using Microsoft.Extensions.Logging;

namespace YcyzClass.Controls.ProfileTransferProviders;

public class YcyzClass1ImportProvider : GenericImportProviderBase
{
    private IProfileService ProfileService { get; } = IAppHost.GetService<IProfileService>();
    
    public YcyzClass1ImportProvider() : base()
    {
        ImportFileHeader = "YcyzClass 1.x 课表文件路径";
        FileTypes =
        [
            new FilePickerFileType("YcyzClass 1.x 课表文件")
            {
                Patterns = ["*.json"]
            }
        ];
        AllowMergeToCurrentProfile = false;
    }
    public override async Task<bool> InvokeTransfer()
    {
        try
        {
            var profile = YcyzClassV1ProfileTransferHelper.TransferYcyzClassV1ProfileToYcyzClassProfile(SourceFilePath);
            if (ImportType == 1)
            {
                var path = Path.Combine(Services.ProfileService.ProfilePath, NewProfileName + ".json");
                if (File.Exists(path))
                {
                    throw new InvalidOperationException($"无法导入课表：{path} 已存在。");
                }
                ConfigureFileHelper.SaveConfig(path, profile);
            }

            this.ShowSuccessToast("导入成功。");
            return true;
        }
        catch (Exception exception)
        {
            var logger = IAppHost.GetService<ILogger<CsesImportProvider>>();
            logger.LogError(exception, "导入 YcyzClass 1.x 课表失败");
            this.ShowErrorToast($"无法导入 YcyzClass 1.x 课表", exception);
            return false;
        }
    }
}