#if false
using YcyzClass.Core.Abstractions.Controls;
using YcyzClass.Core.Attributes;
using YcyzClass.Core.Enums.SettingsWindow;

namespace YcyzClass.Views.SettingPages;

/// <summary>
/// DebugBrushesSettingsPage.xaml 的交互逻辑
/// </summary>
[SettingsPageInfo("debug_brushes", "笔刷", MaterialIconKind.BrushOutline, MaterialIconKind.Brush, SettingsPageCategory.Debug)]
public partial class DebugBrushesSettingsPage : SettingsPageBase
{
    public DebugBrushesSettingsPage()
    {
        InitializeComponent();
        DataContext = this;
    }
}
#endif
