using System;
using YcyzClass.Core.Abstractions.Services;
using YcyzClass.Core.Abstractions.Services.Management;
using YcyzClass.Platforms.Abstraction.Services;
using YcyzClass.Services;
using CommunityToolkit.Mvvm.ComponentModel;

namespace YcyzClass.ViewModels.SettingsPages;

public partial class DebugPageViewModel(
    SettingsService settingsService,
    IManagementService managementService,
    ConsoleService consoleService,
    ILessonsService lessonsService,
    IProfileAnalyzeService profileAnalyzeService,
    ILocationService locationService) : ObservableRecipient
{
    public SettingsService SettingsService { get; } = settingsService;
    public IManagementService ManagementService { get; } = managementService;
    public ConsoleService ConsoleService { get; } = consoleService;
    public ILessonsService LessonsService { get; } = lessonsService;
    public IProfileAnalyzeService ProfileAnalyzeService { get; } = profileAnalyzeService;
    public ILocationService LocationService { get; } = locationService;
    
    [ObservableProperty] private DateTime _targetDate = DateTime.Now;
    
    [ObservableProperty] private TimeSpan _targetTime = TimeSpan.Zero;

    [ObservableProperty] private bool _isTargetDateLoaded = false;

    [ObservableProperty] private bool _isTargetTimeLoaded = false;
    
    public bool IsTargetDateTimeLoaded => IsTargetDateLoaded && IsTargetTimeLoaded;
    
}