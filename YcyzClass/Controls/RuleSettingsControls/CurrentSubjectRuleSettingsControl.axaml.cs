using System;
using YcyzClass.Core.Abstractions.Controls;
using YcyzClass.Core.Abstractions.Services;
using YcyzClass.Core.Abstractions.Services.Management;
using YcyzClass.Core.ComponentModels;
using YcyzClass.Models.Rules;
using YcyzClass.Services;
using YcyzClass.Shared.Models.Profile;
using YcyzClass.ViewModels;

namespace YcyzClass.Controls.RuleSettingsControls;

/// <summary>
/// CurrentSubjectRuleSettingsControl.xaml 的交互逻辑
/// </summary>
public partial class CurrentSubjectRuleSettingsControl : RuleSettingsControlBase<CurrentSubjectRuleSettings>
{
    public ProfileSettingsViewModel ProfileSettingsViewModel { get; }

    public CurrentSubjectRuleSettingsControl(ProfileSettingsViewModel vm)
    {
        ProfileSettingsViewModel = vm;
        InitializeComponent();
    }
}
