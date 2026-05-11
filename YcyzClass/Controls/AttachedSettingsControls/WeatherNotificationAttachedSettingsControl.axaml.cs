using YcyzClass.Core.Abstractions.Controls;
using YcyzClass.Core.Attributes;
using YcyzClass.Core.Enums;
using YcyzClass.Models.AttachedSettings;

namespace YcyzClass.Controls.AttachedSettingsControls;

/// <summary>
/// WeatherNotificationAttachedSettingsControl.xaml 的交互逻辑
/// </summary>
[AttachedSettingsUsage(AttachedSettingsTargets.TimePoint)]
[AttachedSettingsControlInfo("7625DE96-38AA-4B71-B478-3F156DD9458D", "天气提醒设置", "\ue4dc", false)]
public partial class WeatherNotificationAttachedSettingsControl : AttachedSettingsControlBase<WeatherNotificationAttachedSettings>
{
    public WeatherNotificationAttachedSettingsControl()
    {
        InitializeComponent();
    }
}
