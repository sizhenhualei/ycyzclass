using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using YcyzClass.Core.Abstractions.Services;
using YcyzClass.Core.Attributes;
using YcyzClass.Core.Models.Components;
using YcyzClass.Core.Services.Registry;
using YcyzClass.Services;
using YcyzClass.Views;
using YcyzClass.Views.SettingPages;
using CommunityToolkit.Mvvm.ComponentModel;

namespace YcyzClass.ViewModels.SettingsPages;

public partial class ComponentsSettingsViewModel : ObservableRecipient
{
    public IComponentsService ComponentsService { get; }
    public SettingsService SettingsService { get; }

    public ComponentsSettingsPageDropHandler DropHandler { get; }
    
    [ObservableProperty] private ComponentSettings? _selectedComponentSettings;
    [ObservableProperty] private string _createProfileName = "";
    [ObservableProperty] private int _settingsTabControlIndex = 0;
    [ObservableProperty] private bool _isComponentSettingsVisible = false;
    [ObservableProperty] private bool _isComponentAdvancedSettingsVisible = false;
    [ObservableProperty] private ObservableCollection<ComponentSettings> _selectedComponentContainerChildren = new();
    [ObservableProperty] private bool _isComponentChildrenViewOpen = false;
    [ObservableProperty] private bool _isSelectedInChildrenListBox = false;
    [ObservableProperty] private ComponentSettings? _selectedComponentSettingsMain;
    [ObservableProperty] private ComponentSettings? _selectedComponentSettingsChild;
    [ObservableProperty] private Stack<ComponentSettings> _childrenComponentSettingsNavigationStack = new();
    [ObservableProperty] private ComponentSettings? _selectedContainerComponent;
    [ObservableProperty] private bool _canChildrenNavigateBack = false;
    [ObservableProperty] private ComponentSettings? _selectedRootComponent;
    [ObservableProperty] private MainWindowLineSettings? _selectedMainWindowLineSettings;
    [ObservableProperty] private Dictionary<MainWindowLineSettings, ListBox> _mainWindowLineListBoxCache = new();
    [ObservableProperty] private Dictionary<ListBox, MainWindowLineSettings> _mainWindowLineListBoxCacheReversed = new();
    [ObservableProperty] private bool _isSelectedComponentOnRoot = false;

    public static FuncValueConverter<int, double> ComponentsEditorHeightConverter { get; } = new(x => x * 43.2);

    public IReadOnlyList<ComponentInfo> ContainerComponents { get; } =
        ComponentRegistryService.Registered.Where(x => x.IsComponentContainer).ToList();

    /// <inheritdoc/>
    public ComponentsSettingsViewModel(IComponentsService componentsService, SettingsService settingsService)
    {
        ComponentsService = componentsService;
        SettingsService = settingsService;
        DropHandler = new ComponentsSettingsPageDropHandler(this);
    }
}