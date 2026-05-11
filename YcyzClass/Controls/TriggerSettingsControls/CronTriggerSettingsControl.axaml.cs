using YcyzClass.Core.Abstractions.Controls;
using YcyzClass.Models.Automation.Triggers;

namespace YcyzClass.Controls.TriggerSettingsControls;

/// <summary>
/// CronTriggerSettingsControl.axaml 的交互逻辑
/// </summary>
public partial class CronTriggerSettingsControl : TriggerSettingsControlBase<CronTriggerSettings>
{
    public CronTriggerSettingsControl()
    {
        InitializeComponent();
    }
}