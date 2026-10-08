using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using YcyzClass.Core.Abstractions.Services;
using YcyzClass.Core.ComponentModels;
using YcyzClass.Core.Models.Plugin;
using YcyzClass.Services;
using YcyzClass.Shared.ComponentModels;
using YcyzClass.Views.SettingPages;
using CommunityToolkit.Mvvm.ComponentModel;
using DynamicData;
using DynamicData.Binding;
using Microsoft.Extensions.Logging;
using ReactiveUI;

namespace YcyzClass.ViewModels.SettingsPages;

public partial class PluginsSettingsPageViewModel : ObservableRecipient
{
    public IPluginService PluginService { get; }
    public SettingsService SettingsService { get; }
    public ILogger<PluginsSettingsPage> Logger { get; }
    
    [ObservableProperty] private PluginInfo? _selectedPluginInfo;
    [ObservableProperty] private string _readmeDocument = "";
    [ObservableProperty] private bool _isPluginOperationsPopupOpened = false;
    [ObservableProperty] private string _pluginFilterText = "";
    [ObservableProperty] private bool _isLoadingDocument = false;
    [ObservableProperty] private bool _isInstallingLocalPlugin = false;
    [ObservableProperty] private bool _isDetailsShown = false;
    [ObservableProperty] private bool _isDragEntering = false;
    [ObservableProperty] private bool _isDragInstallValid = false;
    [ObservableProperty] private int _dragInstallTotalCount = 0;
    [ObservableProperty] private int _dragInstallSupportedCount = 0;
    [ObservableProperty] private string _dragInstallHintText = "将插件拖入到此处，松手即可安装。";
    [ObservableProperty] private string _dragInstallSubHintText = "";
    [ObservableProperty] private bool _pluginListBoxHasItems = false;

    private ReadOnlyObservableCollection<KeyValuePair<string, PluginInfo>> _mergedPluginsFiltered = null!;
    public ReadOnlyObservableCollection<KeyValuePair<string, PluginInfo>> MergedPluginsFiltered => _mergedPluginsFiltered;

    public SyncDictionaryList<string, PluginInfo> MergedPlugins { get; }

    /// <inheritdoc/>
    public PluginsSettingsPageViewModel(IPluginService pluginService, SettingsService settingsService, ILogger<PluginsSettingsPage> logger)
    {
        PluginService = pluginService;
        SettingsService = settingsService;
        Logger = logger;

        var localPlugins = new ObservableDictionary<string, PluginInfo>();
        foreach (var plugin in IPluginService.LoadedPlugins)
        {
            localPlugins[plugin.Manifest.Id] = plugin;
        }

        MergedPlugins = new SyncDictionaryList<string, PluginInfo>(localPlugins, () => "");

        UpdateMergedPlugins();
    }

    public void UpdateMergedPlugins()
    {
        if (MergedPluginsFiltered != null)
            return;

        var pluginFilter = this
            .WhenAnyValue(x => x.PluginFilterText)
            .Select(_ => new Func<KeyValuePair<string, PluginInfo>, bool>(PluginSourceFilter));

        MergedPlugins.List
            .ToObservableChangeSet()
            .Filter(pluginFilter)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out _mergedPluginsFiltered)
            .Subscribe();

        OnPropertyChanged(nameof(MergedPluginsFiltered));
    }

    private bool PluginSourceFilter(KeyValuePair<string, PluginInfo> kvp)
    {
        var info = kvp.Value;
        
        var filter = PluginFilterText;
        if (string.IsNullOrWhiteSpace(filter))
            return true;
        return info.Manifest.Id.Contains(filter, StringComparison.OrdinalIgnoreCase) ||
               info.Manifest.Name.Contains(filter, StringComparison.OrdinalIgnoreCase) ||
               info.Manifest.Description.Contains(filter, StringComparison.OrdinalIgnoreCase);
    }
}
