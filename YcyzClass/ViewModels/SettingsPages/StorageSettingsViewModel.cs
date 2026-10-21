using YcyzClass.Services;
using YcyzClass.Views.SettingPages;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;

namespace YcyzClass.ViewModels.SettingsPages;

public partial class StorageSettingsViewModel(
    FileFolderService fileFolderService,
    SettingsService settingsService,
    ILogger<StorageSettingsPage> logger) : ObservableRecipient
{
    public FileFolderService FileFolderService { get; } = fileFolderService;
    public SettingsService SettingsService { get; } = settingsService;
    public ILogger<StorageSettingsPage> Logger { get; } = logger;
    
    [ObservableProperty] private bool _isBackingUp = false;
    [ObservableProperty] private bool _isBackupFinished = false;
    
}