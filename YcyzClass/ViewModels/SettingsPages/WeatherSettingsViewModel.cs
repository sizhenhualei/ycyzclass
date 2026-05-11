using YcyzClass.Core.Models.Weather;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using YcyzClass.Core.Abstractions.Services;
using YcyzClass.Platforms.Abstraction.Services;
using YcyzClass.Services;
using YcyzClass.Views.SettingPages;
using Microsoft.Extensions.Logging;

namespace YcyzClass.ViewModels.SettingsPages;

public partial class WeatherSettingsViewModel(SettingsService settingsService, 
    ILocationService locationService, 
    IWeatherService weatherService,
    ILogger<WeatherSettingsPage> logger) : ObservableRecipient
{
    public SettingsService SettingsService { get; } = settingsService;
    public ILocationService LocationService { get; } = locationService;
    public IWeatherService WeatherService { get; } = weatherService;
    public ILogger<WeatherSettingsPage> Logger { get; } = logger;

    [ObservableProperty] private List<City> _citySearchResults = new();
    [ObservableProperty] private bool _isSearchingWeather = false;
    [ObservableProperty] private bool _isRefreshingWeather = false;
    [ObservableProperty] private bool _hideLocationPos = true;
    [ObservableProperty] private bool _isLocationUpdating = false;
    
}