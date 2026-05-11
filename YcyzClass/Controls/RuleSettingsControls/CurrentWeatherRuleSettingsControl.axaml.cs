using YcyzClass.Core.Abstractions.Controls;
using YcyzClass.Core.Abstractions.Services;
using YcyzClass.Models.Rules;

namespace YcyzClass.Controls.RuleSettingsControls;

/// <summary>
/// CurrentWeatherRuleSettingsControl.xaml 的交互逻辑
/// </summary>
public partial class CurrentWeatherRuleSettingsControl : RuleSettingsControlBase<CurrentWeatherRuleSettings>
{
    public IWeatherService WeatherService { get; }

    public CurrentWeatherRuleSettingsControl(IWeatherService weatherService)
    {
        WeatherService = weatherService;
        InitializeComponent();
    }
}