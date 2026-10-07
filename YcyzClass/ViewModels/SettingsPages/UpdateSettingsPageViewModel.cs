using YcyzClass.Services;
using YcyzClass.Services.AppUpdating;
using YcyzClass.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using PhainonDistributionCenter.Shared.Models.Client;
using UpdateSettingsPage = YcyzClass.Views.SettingPages.UpdateSettingsPage;

namespace YcyzClass.ViewModels.SettingsPages;

public partial class UpdateSettingsPageViewModel(ILogger<UpdateSettingsPage> logger, UpdateService updateService, SettingsService settingsService) : ObservableObject
{
    public ILogger<UpdateSettingsPage> Logger { get; } = logger;
    public UpdateService UpdateService { get; } = updateService;
    public SettingsService SettingsService { get; } = settingsService;

    [ObservableProperty] private DistributionMetadata.DistributionChannel _selectedChannel = new();
    [ObservableProperty] private string _changeLogDocument = "";
    [ObservableProperty] private string _newVersionChangeLogDocument = "";
}