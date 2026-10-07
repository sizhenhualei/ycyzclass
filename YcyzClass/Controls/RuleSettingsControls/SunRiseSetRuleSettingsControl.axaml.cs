using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using YcyzClass.Core.Abstractions.Controls;
using YcyzClass.Models.Rules;

namespace YcyzClass.Controls.RuleSettingsControls;

public partial class SunRiseSetRuleSettingsControl : RuleSettingsControlBase<SunRiseSetRuleSettings>
{
    public SunRiseSetRuleSettingsControl()
    {
        InitializeComponent();
    }
}
