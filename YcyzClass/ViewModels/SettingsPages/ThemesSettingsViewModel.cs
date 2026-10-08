using System;
using System.Collections.Generic;
using YcyzClass.Core.Abstractions.Services;
using YcyzClass.Core.ComponentModels;
using CommunityToolkit.Mvvm.ComponentModel;
using YcyzClass.Core.Models.XamlTheme;
using YcyzClass.Services;
using DynamicData;
using DynamicData.Binding;

namespace YcyzClass.ViewModels.SettingsPages;

public partial class ThemesSettingsViewModel : ObservableObject
{
    public IXamlThemeService XamlThemeService { get; }
    public SettingsService SettingsService { get; }
    
    [ObservableProperty] private ThemeInfo? _selectedThemeInfo;
    [ObservableProperty] private bool _isThemeOperationsPopupOpened = false;
    [ObservableProperty] private string _themeFilterText = "";
    [ObservableProperty] private bool _isDragEntering = false;
    
    [ObservableProperty] private IObservableList<KeyValuePair<string, ThemeInfo>> _mergedThemesFiltered = null!;

    public SyncDictionaryList<string, ThemeInfo> MergedThemes { get; set; } = null!;
    
    /// <inheritdoc/>
    public ThemesSettingsViewModel(IXamlThemeService xamlThemeService, SettingsService settingsService)
    {
        XamlThemeService = xamlThemeService;
        SettingsService = settingsService;
        
        UpdateMergedThemes();
    }
    
    public void UpdateMergedThemes()
    {
        MergedThemes = new SyncDictionaryList<string, ThemeInfo>(XamlThemeService.MergedThemes, () => "");
        MergedThemesFiltered = MergedThemes.List
            .ToObservableChangeSet()
            .Filter(ThemeSourceFilter)
            .AsObservableList();
    }
    
    private bool ThemeSourceFilter(KeyValuePair<string, ThemeInfo> kvp)
    {
        var info = kvp.Value;
        var filter = ThemeFilterText;
        if (string.IsNullOrWhiteSpace(filter))
            return true;
        return info.Manifest.Id.Contains(filter, StringComparison.OrdinalIgnoreCase) ||
                 info.Manifest.Name.Contains(filter, StringComparison.OrdinalIgnoreCase) ||
                 info.Manifest.Description.Contains(filter, StringComparison.OrdinalIgnoreCase);
    }

}
