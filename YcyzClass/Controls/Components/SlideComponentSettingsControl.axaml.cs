using System.Windows;
using Avalonia.Controls;
using Avalonia.Interactivity;
using YcyzClass.Core.Abstractions.Controls;
using YcyzClass.Core.Controls.Ruleset;
using YcyzClass.Models.ComponentSettings;

namespace YcyzClass.Controls.Components;

/// <summary>
/// SlideComponentSettingsControl.xaml 的交互逻辑
/// </summary>
public partial class SlideComponentSettingsControl : ComponentBase<SlideComponentSettings>
{
    private void OpenDrawer(string key)
    {
        if (this.FindResource(key) is not Control drawer)
        {
            return;
        }

        drawer.DataContext = this;
        SettingsPageBase.OpenDrawerCommand.Execute(drawer);
    }

    public SlideComponentSettingsControl()
    {
        InitializeComponent();
    }

    private void ButtonOpenPauseRuleset_OnClick(object sender, RoutedEventArgs e)
    {
        if (this.FindResource("RulesetControl") is RulesetControl rulesetControl) 
            rulesetControl.Ruleset = Settings.PauseRule;
        OpenDrawer("RulesetControl");
    }

    private void ButtonOpenStopRuleset_OnClick(object sender, RoutedEventArgs e)
    {
        if (this.FindResource("RulesetControl") is RulesetControl rulesetControl)
            rulesetControl.Ruleset = Settings.StopRule;
        OpenDrawer("RulesetControl");
    }
}
