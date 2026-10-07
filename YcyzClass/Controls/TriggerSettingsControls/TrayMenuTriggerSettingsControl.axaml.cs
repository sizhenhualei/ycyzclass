using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using YcyzClass.Core.Abstractions.Controls;
using YcyzClass.Models.Automation.Triggers;

namespace YcyzClass.Controls.TriggerSettingsControls;

public partial class TrayMenuTriggerSettingsControl : TriggerSettingsControlBase<TrayMenuTriggerSettings>
{
    public TrayMenuTriggerSettingsControl()
    {
        InitializeComponent();
    }
}