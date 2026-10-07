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

public class ClassWidgets1ImportProvider : GenericImportProviderBase
{
    private IProfileService ProfileService { get; } = IAppHost.GetService<IProfileService>();
    
    public ClassWidgets1ImportProvider() : base()
    {
        ImportFileHeader = "Class Widgets 1 课表文件路径";
        FileTypes =
        [
            new FilePickerFileType("Class Widgets 1 课表文件")
            {
                Patterns = ["*.json"]
            }
        ];
    }
    public override async Task<bool> InvokeTransfer()
    {
        try
        {
            var profile = ClassWidgetsProfileTransferHelper.ConvertClassWidgets1ProfileToYcyzClassProfile(SourceFilePath, ImportType == 0 ? ProfileService.Profile : null);
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
            logger.LogError(exception, "导入 Class Widgets 1 课表失败");
            this.ShowErrorToast($"无法导入 Class Widgets 1 课表", exception);
            return false;
        }
    }
}