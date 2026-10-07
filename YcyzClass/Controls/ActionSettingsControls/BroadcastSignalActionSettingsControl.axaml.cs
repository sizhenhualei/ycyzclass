using YcyzClass.Core.Abstractions.Controls;
using YcyzClass.Models.Automation.Triggers;

namespace YcyzClass.Controls.ActionSettingsControls;

/// <summary>
/// BroadcastSignalActionSettingsControl.xaml 的交互逻辑
/// </summary>
public partial class BroadcastSignalActionSettingsControl : ActionSettingsControlBase<SignalTriggerSettings>
{
    public BroadcastSignalActionSettingsControl()
    {
        InitializeComponent();
    }
}
