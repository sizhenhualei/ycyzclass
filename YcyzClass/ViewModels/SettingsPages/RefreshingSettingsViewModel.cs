using YcyzClass.Core.Abstractions.Services;
using YcyzClass.Services;
using CommunityToolkit.Mvvm.ComponentModel;

namespace YcyzClass.ViewModels.SettingsPages;

public partial class RefreshingSettingsViewModel(
    SettingsService settingsService, 
    ITutorialService tutorialService,
    IRefreshingService refreshingService) : ObservableObject
{
    public SettingsService SettingsService { get; } = settingsService;
    public ITutorialService TutorialService { get; } = tutorialService;
    public IRefreshingService RefreshingService { get; } = refreshingService;
}