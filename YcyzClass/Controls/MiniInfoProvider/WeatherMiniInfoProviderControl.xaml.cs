#if false
using System.Windows.Controls;

using YcyzClass.Models;
using YcyzClass.Services;

namespace YcyzClass.Controls.MiniInfoProvider;

/// <summary>
/// WeatherMiniInfoProviderControl.xaml 的交互逻辑
/// </summary>
public partial class WeatherMiniInfoProviderControl : UserControl
{
    private SettingsService SettingsService { get; }

    public Settings AppSettings => SettingsService.Settings;

    public WeatherMiniInfoProviderSettings Settings { get; }

    public WeatherMiniInfoProviderControl(SettingsService settingsService, WeatherMiniInfoProviderSettings weatherMiniInfoProviderSettings)
    {
        Settings = weatherMiniInfoProviderSettings;
        SettingsService = settingsService;
        InitializeComponent();
    }
}
#endif
