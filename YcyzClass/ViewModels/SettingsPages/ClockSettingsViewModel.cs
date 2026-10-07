using System;
using YcyzClass.Core.Abstractions.Services;
using YcyzClass.Services;
using CommunityToolkit.Mvvm.ComponentModel;

namespace YcyzClass.ViewModels.SettingsPages;

public partial class ClockSettingsViewModel(
    SettingsService settingsService,
    IExactTimeService exactTimeService,
    ILessonsService lessonsService) : ObservableObject
{
    public SettingsService SettingsService { get; } = settingsService;
    public IExactTimeService ExactTimeService { get; } = exactTimeService;
    public ILessonsService LessonsService { get; } = lessonsService;

    [ObservableProperty] private DateTime _currentTime = DateTime.Now;
}