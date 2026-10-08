using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices.JavaScript;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Platform;
using Avalonia.Platform.Storage;
using YcyzClass.Core;
using YcyzClass.Core.Abstractions.Services;
using YcyzClass.Core.Helpers.UI;
using YcyzClass.Core.Models.Components;
using YcyzClass.Enums;
using YcyzClass.Helpers.ProfileTransferHelpers;
using YcyzClass.Models;
using YcyzClass.Models.External.ClassWidgets;
using YcyzClass.Models.NotificationProviderSettings;
using YcyzClass.Platforms.Abstraction;
using YcyzClass.Platforms.Abstraction.Models;
using YcyzClass.Services;
using YcyzClass.Services.Logging;
using YcyzClass.Services.Management;
using YcyzClass.Services.NotificationProviders;
using YcyzClass.Shared;
using YcyzClass.Shared.Helpers;
using YcyzClass.Shared.Models.Notification;
using YcyzClass.Shared.Models.Profile;
using YcyzClass.ViewModels;
using FluentAvalonia.UI.Controls;
using IniParser;
using IniParser.Model;
using Microsoft.Extensions.Logging;

namespace YcyzClass.Views;

public partial class DataTransferPage : UserControl
{
    public DataTransferViewModel ViewModel { get; } = IAppHost.GetService<DataTransferViewModel>();
    
    public DataTransferPage()
    {
        InitializeComponent();
    }
    
    private async void SettingsExpanderBrowse_OnClick(object? sender, RoutedEventArgs e)
    {
        
        if (ViewModel.BrowseAction == null || ViewModel.PerformImportAction == null)
        {
            return;
        }
        await ViewModel.BrowseAction();
        if (string.IsNullOrWhiteSpace(ViewModel.ImportSourcePath))
        {
            return;
        }

        await ViewModel.PerformImportAction();
    }
    
    private void ButtonTurnBack_OnClick(object? sender, RoutedEventArgs e)
    {
        ViewModel.PageIndex--;
    }

    #region YcyzClass
    
    private async void SettingsExpanderImportFromClassIsland1_OnClick(object? sender, RoutedEventArgs e)
    {
        ViewModel.BrowseAction = BrowseYcyzClassData;
        ViewModel.PerformImportAction = BeginPerformClassIslandImport;
        ViewModel.PageIndex = 1;
        ViewModel.ImportSourcePath = "";
        ViewModel.ImportDescription = "支持从 1.x 版本的 ClassIsland 导入课表、组件配置、自动化配置、应用设置、部分插件和主题等数据。";
        ViewModel.IsExport = false;
    }

    private async Task BrowseYcyzClassData()
    {
        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel == null)
        {
            return;
        }

        PopupHelper.DisableAllPopups();
        var file = await PlatformServices.FilePickerService.OpenFilesPickerAsync(new FilePickerOpenOptions()
        {
            Title = "选择先前版本的 ClassIsland 实例",
            FileTypeFilter =
            [
                new FilePickerFileType("ClassIsland 可执行文件")
                {
                    Patterns = ["ClassIsland.exe"]
                }
            ]
        }, topLevel);
        PopupHelper.RestoreAllPopups();
        if (file.Count <= 0)
        {
            return;
        }

        ViewModel.ImportSourcePath = Path.GetDirectoryName(file[0]) ?? "";
    }

    private async Task BeginPerformClassIslandImport()
    {
        var r = await new ContentDialog()
        {
            Title = "重启以继续",
            Content = "应用需要重启以继续导入操作，要重启以继续导入吗？",
            PrimaryButtonText = "重启并继续",
            SecondaryButtonText = "取消",
            DefaultButton = ContentDialogButton.Primary
        }.ShowAsync(TopLevel.GetTopLevel(this));
        if (r != ContentDialogResult.Primary)
        {
            return;
        }

        var entry = ImportEntries.None;
        if (ViewModel.IsProfileSelected)
        {
            entry |= ImportEntries.Profiles;
        }
        if (ViewModel.IsSettingsSelected)
        {
            entry |= ImportEntries.Settings;
        }
        if (ViewModel.IsOtherConfigSelected)
        {
            entry |= ImportEntries.OtherConfig;
        }
        AppBase.Current.Restart(["--importV1", ViewModel.ImportSourcePath, "-m", "--importEntries", ((int)entry).ToString()]);
    }

    private void ImportSettings(string root)
    {
        var settings = ConfigureFileHelper.LoadConfigUnWrapped<Settings>(Path.Combine(root, "Settings.json"), false);
        if (settings.LastAppVersion < Version.Parse("1.7.0.0"))
        {
            throw new Exception("源 ClassIsland 版本必须在 1.7.0.x，才能进行导入。");
        }
        settings.MainWindowFont = MainWindow.DefaultFontFamilyKey;
        settings.ShowEchoCaveWhenSettingsPageLoading = false;
        if (settings.WeatherIconId == "ycyzclass.weatherIcons.materialDesign")
        {
            settings.WeatherIconId = "ycyzclass.weatherIcons.lucide";
        }

        if (PlatformServices.DesktopService.IsAutoStartEnabled)
        {
            PlatformServices.DesktopService.IsAutoStartEnabled = false;
            PlatformServices.DesktopService.IsAutoStartEnabled = true;
        }
        if (PlatformServices.DesktopService.IsUrlSchemeRegistered)
        {
            PlatformServices.DesktopService.IsUrlSchemeRegistered = false;
            PlatformServices.DesktopService.IsUrlSchemeRegistered = true;
        }
        ConfigureFileHelper.SaveConfig(Path.Combine(CommonDirectories.AppRootFolderPath, "Settings.json"), settings);
    }
    
    private void ImportConfig(string root)
    {
        Directory.Delete(Path.Combine(CommonDirectories.AppRootFolderPath, "Plugins"), true);
        
        FileFolderService.CopyFolder(Path.Combine(root, "Config"),
            Path.Combine(CommonDirectories.AppRootFolderPath, "Config"), true);
        FileFolderService.CopyFolder(Path.Combine(root, "Plugins"),
            Path.Combine(CommonDirectories.AppRootFolderPath, "Plugins"), true);
        foreach (var plugin in Directory.GetDirectories(Path.Combine(CommonDirectories.AppRootFolderPath, "Plugins")))
        {
            File.WriteAllText(Path.Combine(plugin, ".disabled"), "");
        }
        if (!Directory.Exists(ComponentsService.ComponentSettingsPath))
        {
            Directory.CreateDirectory(ComponentsService.ComponentSettingsPath);
        }
        foreach (var path in Directory.GetFiles(Path.Combine(CommonDirectories.AppConfigPath, "Islands")))
        {
            var oldConfig =
                ConfigureFileHelper.LoadConfigUnWrapped<ObservableCollection<ComponentSettings>>(path, false);
            var newConfig = new ComponentProfile();
            var e = oldConfig.OrderBy(x => x.RelativeLineNumber)
                .GroupBy(x => x.RelativeLineNumber)
                .Select(x => new MainWindowLineSettings()
                {
                    IsMainLine = x.Key == 0,
                    Children = new ObservableCollection<ComponentSettings>(x)
                });
            newConfig.Lines = new ObservableCollection<MainWindowLineSettings>(e);
            ConfigureFileHelper.SaveConfig(Path.Combine(ComponentsService.ComponentSettingsPath, Path.GetFileName(path)), newConfig);
        }
    }
    
    [Obsolete]
    private void ImportProfile(string root)
    {
        FileFolderService.CopyFolder(Path.Combine(root, "Profiles"),
            Path.Combine(CommonDirectories.AppRootFolderPath, "Profiles"), true);
        foreach (var path in Directory.GetFiles(ProfileService.ProfilePath))
        {
            var config = ClassIslandV1ProfileTransferHelper.TransferClassIslandV1ProfileToYcyzClassProfile(path);
            ConfigureFileHelper.SaveConfig(Path.Combine(ProfileService.ProfilePath, Path.GetFileName(path)), config);
        }
    }

    [Obsolete]
    public async Task PerformClassIslandImport(string root, ImportEntries importEntries)
    {
        try
        {
            ViewModel.PageIndex = 3;
            await Task.Run(() =>
            {
                if ((importEntries & ImportEntries.Settings) == ImportEntries.Settings)
                {
                    ImportSettings(root);
                }
                if ((importEntries & ImportEntries.OtherConfig) == ImportEntries.OtherConfig)
                {
                    ImportConfig(root);
                }

                if ((importEntries & ImportEntries.Profiles) == ImportEntries.Profiles)
                {
                    ImportProfile(root);
                }
            });
            
            AppBase.Current.Restart(["-m", "--importComplete"]);
        }
        catch (Exception e)
        {
            IAppHost.TryGetService<ILogger<DataTransferPage>>()?.LogError(e, "导入时发生意外错误");
            this.ShowErrorToast("导入时发生意外错误", e);
        }

    }

    #endregion

    #region ClassIsland 2 / Import

    private void SettingsExpanderImportFromClassIsland2_OnClick(object? sender, RoutedEventArgs e)
    {
        ViewModel.BrowseAction = BrowseClassIsland2Data;
        ViewModel.PerformImportAction = BeginPerformClassIsland2Import;
        ViewModel.PageIndex = 1;
        ViewModel.ImportSourcePath = "";
        ViewModel.ImportDescription = "支持从 ClassIsland 2.x 数据文件导入课表、组件配置、自动化配置、应用设置、部分插件和主题等数据。";
        ViewModel.IsExport = false;
    }
    
    private async Task BrowseClassIsland2Data()
    {
        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel == null)
        {
            return;
        }

        PopupHelper.DisableAllPopups();
        var file = await PlatformServices.FilePickerService.OpenFilesPickerAsync(new FilePickerOpenOptions()
        {
            Title = "浏览 ClassIsland 2 导出文件",
            FileTypeFilter = [ new FilePickerFileType("YcyzClass 数据文件")
                {
                    Patterns = ["*.cidata"]
                } 
            ]
        }, topLevel);
        PopupHelper.RestoreAllPopups();
        if (file.Count <= 0)
        {
            return;
        }

        ViewModel.ImportSourcePath = file[0] ?? "";
    }
    
    private async Task BeginPerformClassIsland2Import()
    {
        var r = await new ContentDialog()
        {
            Title = "重启以继续",
            Content = "应用需要重启以继续导入操作，要重启以继续导入吗？",
            PrimaryButtonText = "重启并继续",
            SecondaryButtonText = "取消",
            DefaultButton = ContentDialogButton.Primary
        }.ShowAsync(TopLevel.GetTopLevel(this));
        if (r != ContentDialogResult.Primary)
        {
            return;
        }

        var entry = ImportEntries.None;
        if (ViewModel.IsProfileSelected)
        {
            entry |= ImportEntries.Profiles;
        }
        if (ViewModel.IsSettingsSelected)
        {
            entry |= ImportEntries.Settings;
        }
        if (ViewModel.IsOtherConfigSelected)
        {
            entry |= ImportEntries.OtherConfig;
        }
        AppBase.Current.Restart(["--importV2", ViewModel.ImportSourcePath, "-m", "--importEntries", ((int)entry).ToString()]);
    }
    
    public async Task PerformClassIsland2Import(string root, ImportEntries importEntries)
    {
        try
        {
            ViewModel.PageIndex = 3;
            await Task.Run(() =>
            {
                var appRoot = Path.GetFullPath(CommonDirectories.AppRootFolderPath);
                Directory.CreateDirectory(appRoot);
                if (importEntries.HasFlag(ImportEntries.OtherConfig) && Directory.Exists(PluginService.PluginsRootPath))
                {
                    Directory.Delete(PluginService.PluginsRootPath, true);
                }

                using var archive = ZipFile.OpenRead(root);
                foreach (var entry in archive.Entries)
                {
                    if (!ShouldImportEntry(entry.FullName))
                    {
                        continue;
                    }

                    ExtractEntry(entry, appRoot);
                }

                return;

                bool ShouldImportEntry(string entryName)
                {
                    var normalizedName = entryName.Replace('\\', '/');
                    if ((importEntries & ImportEntries.Settings) == ImportEntries.Settings &&
                        normalizedName == "Settings.json")
                    {
                        return true;
                    }

                    if ((importEntries & ImportEntries.Profiles) == ImportEntries.Profiles &&
                        normalizedName.StartsWith("Profiles/", StringComparison.Ordinal))
                    {
                        return true;
                    }

                    if ((importEntries & ImportEntries.OtherConfig) != ImportEntries.OtherConfig)
                    {
                        return false;
                    }

                    return normalizedName.StartsWith("Config/", StringComparison.Ordinal) ||
                           normalizedName.StartsWith("Plugins/", StringComparison.Ordinal);
                }

                static void ExtractEntry(ZipArchiveEntry entry, string appDataRoot)
                {
                    var targetPath = Path.GetFullPath(Path.Combine(appDataRoot,
                        entry.FullName.Replace('/', Path.DirectorySeparatorChar)));
                    var rootWithSeparator = appDataRoot.EndsWith(Path.DirectorySeparatorChar)
                        ? appDataRoot
                        : appDataRoot + Path.DirectorySeparatorChar;
                    if (!targetPath.StartsWith(rootWithSeparator, StringComparison.Ordinal) && targetPath != appDataRoot)
                    {
                        throw new InvalidDataException($"压缩包包含无效路径：{entry.FullName}");
                    }

                    if (string.IsNullOrEmpty(entry.Name))
                    {
                        Directory.CreateDirectory(targetPath);
                        return;
                    }

                    Directory.CreateDirectory(Path.GetDirectoryName(targetPath) ?? appDataRoot);
                    entry.ExtractToFile(targetPath, true);
                }
            });
            
            AppBase.Current.Restart(["-m", "--importComplete"]);
        }
        catch (Exception e)
        {
            IAppHost.TryGetService<ILogger<DataTransferPage>>()?.LogError(e, "导入时发生意外错误");
            this.ShowErrorToast("导入时发生意外错误", e);
        }

    }

    #endregion    
    
    #region ClassIsland 2 / Export

    private void SettingsExpanderExportToClassIsland2_OnClick(object? sender, RoutedEventArgs e)
    {
        ViewModel.BrowseAction = BrowseClassIsland2ExportTargetAction;
        ViewModel.PerformImportAction = PerformClassIsland2ExportAction;
        ViewModel.PageIndex = 1;
        ViewModel.ImportSourcePath = "";
        ViewModel.ImportDescription = "支持导出为可以导入到其它 ClassIsland 2 实例的文件格式。";
        ViewModel.IsExport = true;
    }


    private async Task BrowseClassIsland2ExportTargetAction()
    {
        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel == null)
        {
            return;
        }

        PopupHelper.DisableAllPopups();
        var file = await PlatformServices.FilePickerService.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "浏览保存的导出文件",
            FileTypeChoices = [ new FilePickerFileType("YcyzClass 数据文件")
                {
                    Patterns = ["*.cidata"]
                } 
            ],
            SuggestedFileName = $"YcyzClass_data_{AppBase.AppVersion}_{DateTime.Now:yyyyMMdd_HHmmss}.cidata"
        }, topLevel);
        PopupHelper.RestoreAllPopups();
        
        
        ViewModel.ImportSourcePath = file ?? "";
    }

    private async Task PerformClassIsland2ExportAction()
    {
        if (string.IsNullOrWhiteSpace(ViewModel.ImportSourcePath))
        {
            return;
        }
        try
        {
            ViewModel.PageIndex = 3;
            var path = ViewModel.ImportSourcePath;
            var temp = Directory.CreateTempSubdirectory("YcyzClassDataExport").FullName;
            //await File.WriteAllTextAsync(Path.Combine(temp, "Logs.log"), logs);
            Directory.CreateDirectory(Path.Combine(temp, "Profiles/"));
            Directory.CreateDirectory(Path.Combine(temp, "Plugins/"));
            Directory.CreateDirectory(Path.Combine(temp, "Config/"));
            
            await Task.Run(() =>
            {
                if (ViewModel.IsSettingsSelected)
                {
                    File.Copy(Path.Combine(CommonDirectories.AppRootFolderPath, "Settings.json"), Path.Combine(temp, "Settings.json"));
                }
                if (ViewModel.IsProfileSelected)
                {
                    FileFolderService.CopyFolder(Path.Combine(CommonDirectories.AppRootFolderPath, "./Profiles"), Path.Combine(temp, "Profiles/"));
                }
                if (ViewModel.IsOtherConfigSelected)
                {
                    FileFolderService.CopyFolder(Path.Combine(CommonDirectories.AppConfigPath), Path.Combine(temp, "Config/"));
                    FileFolderService.CopyFolder(Path.Combine(PluginService.PluginsRootPath), Path.Combine(temp, "Plugins/"));
                }
                File.Delete(path);
                ZipFile.CreateFromDirectory(temp, path);
            });
            Directory.Delete(temp, true);
            ViewModel.PageIndex = 4;
        }
        catch (Exception e)
        {
            ViewModel.Logger.LogError(e, "导出数据时发生意外错误。");
            this.ShowErrorToast("导出数据时发生意外错误。", e);
            throw;
        }
    }
    #endregion
    

    #region Class Widgets

    private double TryGetDoubleFromSection(PropertyCollection? dictionary, string key, double fallback) 
    {
        if (dictionary == null)
        {
            return fallback;
        }
        
        return dictionary[key] != null ? (double.TryParse(dictionary[key], out var r2) ? r2 : fallback) : fallback;
    }
    
    private bool TryGetBooleanFromSection(PropertyCollection? dictionary, string key, bool fallback) 
    {
        if (dictionary == null)
        {
            return fallback;
        }

        return dictionary[key] != null ? (int.TryParse(dictionary[key], out var r2) ? r2 == 1 : fallback) : fallback;
    }
    
    private string TryGetStringFromSection(PropertyCollection? dictionary, string key, string fallback) 
    {
        if (dictionary == null)
        {
            return fallback;
        }

        return dictionary[key] != null ? dictionary[key] : fallback;
    }
    
    private int TryGetIntFromSection(PropertyCollection? dictionary, string key, int fallback) 
    {
        if (dictionary == null)
        {
            return fallback;
        }

        return dictionary[key] != null ? (int.TryParse(dictionary[key], out var r2) ? r2 : fallback) : fallback;
    }

    private void ImportFromClassWidgets1_OnClick(object? sender, RoutedEventArgs e)
    {
        ViewModel.BrowseAction = BrowseClassWidgetsData;
        ViewModel.PerformImportAction = PerformClassWidgetsImportAction;
        ViewModel.PageIndex = 1;
        ViewModel.ImportSourcePath = "";
        ViewModel.ImportDescription = "支持从 Class Widgets 1 导入全部课表信息和大部分配置。";
        ViewModel.IsExport = false;
    }
    
    private async Task BrowseClassWidgetsData()
    {
        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel == null)
        {
            return;
        }

        PopupHelper.DisableAllPopups();
        var file = await PlatformServices.FilePickerService.OpenFoldersPickerAsync(new FolderPickerOpenOptions()
        {
            Title = "浏览 Class Widgets 安装（即有 ClassWidgets[.exe]）的文件夹或其数据目录"
        }, topLevel);
        PopupHelper.RestoreAllPopups();
        if (file.Count <= 0)
        {
            return;
        }
        
        ViewModel.ImportSourcePath = file[0];
    }

    private void ImportClassWidgetsProfile(string root, IniData ini)
    {
        var general = ini["General"];
        var profileName = TryGetStringFromSection(general, "schedule", "新课表 - 1.json");

        var profile =
            ClassWidgetsProfileTransferHelper.ConvertClassWidgets1ProfileToYcyzClassProfile(Path.Combine(root,
                "config", "schedule", profileName));
        
        ConfigureFileHelper.SaveConfig(Path.Combine(ProfileService.ProfilePath, $"cw_{profileName}"), profile);
    }
    
    private void ImportClassWidgetsSettings(string root, IniData ini)
    {
        var settingsService = IAppHost.GetService<SettingsService>();
        var settings = settingsService.Settings;
        
        // General
        var general = ini["General"];
        settings.Scale = TryGetDoubleFromSection(general, "scale", 1.0);
        settings.Opacity = TryGetDoubleFromSection(general, "opacity", 100) / 100;
        settings.HideOnClass = TryGetIntFromSection(general, "hide", 0) == 1;
        PlatformServices.DesktopService.IsAutoStartEnabled = TryGetBooleanFromSection(general, "auto_startup", false);
        settings.WindowLayer = TryGetIntFromSection(general, "pin_on_top", 0) switch
        {
            0 or 2 => 0,
            1 => 1,
            _ => 0
        };
        settings.Theme = TryGetIntFromSection(general, "color_mode", 2) switch
        {
            2 => 0,
            0 => 1,
            1 => 2,
            _ => 0
        };
        
        // Weather
        var weather = ini["Weather"];
        if (TryGetStringFromSection(weather, "api", "xiaomi_weather") == "xiaomi_weather")
        {
            settings.CityId = TryGetStringFromSection(weather, "city", "weathercn:101010100");
            settings.WeatherLocationSource = 0;
        }
        
        // Toast
        var toast = ini["Toast"];
        settings.IsNotificationEnabled = true;
        settings.IsNotificationEffectEnabled =
            settings.AllowNotificationEffect = TryGetBooleanFromSection(toast, "wave", true);
        settings.IsNotificationSoundEnabled =
            settings.AllowNotificationSound = TryGetBooleanFromSection(toast, "ringtone", true);
        var classSettings =
            settings.NotificationProvidersSettings.GetValueOrDefault("08F0D9C3-C770-4093-A3D0-02F3D90C24BC".ToLower()) as ClassNotificationSettings
            ?? new ClassNotificationSettings();
        classSettings.InDoorClassPreparingDeltaTime = classSettings.OutDoorClassPreparingDeltaTime
            = (int)(TryGetDoubleFromSection(toast, "prepare_minutes", 2.0) * 60);
        classSettings.IsClassOnNotificationEnabled = TryGetBooleanFromSection(toast, "attend_class", true);
        classSettings.IsClassOffNotificationEnabled = TryGetBooleanFromSection(toast, "finish_class", true);
        classSettings.IsClassOnPreparingNotificationEnabled = TryGetBooleanFromSection(toast, "prepare_class", true);
        settings.NotificationProvidersSettings["08F0D9C3-C770-4093-A3D0-02F3D90C24BC".ToLower()] = classSettings;
        var afterSchoolSettings =
            settings.NotificationProvidersSettings.GetValueOrDefault("8FBC3A26-6D20-44DD-B895-B9411E3DDC51".ToLower()) as AfterSchoolNotificationProviderSettings
            ?? new AfterSchoolNotificationProviderSettings();
        afterSchoolSettings.IsEnabled = TryGetBooleanFromSection(toast, "after_school", true);
        settings.NotificationProvidersSettings["8FBC3A26-6D20-44DD-B895-B9411E3DDC51".ToLower()] = afterSchoolSettings;
        
        // Time
        var time = ini["Time"];
        settings.IsExactTimeEnabled = TryGetStringFromSection(time, "type", "ntp") == "ntp";
        settings.ExactTimeServer = TryGetStringFromSection(time, "ntp_server", "ntp.aliyun.com");
        settings.TimeOffsetSeconds = TryGetDoubleFromSection(time, "time_offset", 0);
        
        // Date
        var date = ini["Date"];
        settings.SingleWeekStartTime = DateOnly.TryParse(TryGetStringFromSection(date, "start_date", ""), out var d)
            ? d.ToDateTime(TimeOnly.MinValue)
            : DateTime.Now;
        
        // Audio
        var audio = ini["Audio"];
        settings.NotificationSoundVolume = TryGetDoubleFromSection(audio, "volume", 100) / 100.0;
        SetNotificationChannelSound(ClassNotificationProvider.OnClassChannelId,
            TryGetStringFromSection(audio, "attend_class", "attend_class.wav"));
        SetNotificationChannelSound(ClassNotificationProvider.OnBreakingChannelId,
            TryGetStringFromSection(audio, "finish_class", "finish_class.wav"));
        SetNotificationChannelSound(ClassNotificationProvider.PrepareOnClassChannelId,
            TryGetStringFromSection(audio, "prepare_class", "prepare_class.wav"));

        settings.IsWelcomeWindowShowed = true;

        return;
        
        void SetNotificationChannelSound(string channelId, string cwPath)
        {
            var chanelSettings =
                settings.NotificationChannelsNotifySettings.GetValueOrDefault(channelId) ?? new NotificationSettings();
            chanelSettings.IsNotificationSoundEnabled = true;
            chanelSettings.IsSettingsEnabled = true;
            if (!Directory.Exists(Path.Combine(CommonDirectories.AppConfigPath, "ExternalAudios")))
            {
                Directory.CreateDirectory(Path.Combine(CommonDirectories.AppConfigPath, "ExternalAudios"));
            }
            var ciPath = Path.GetFullPath(Path.Combine(CommonDirectories.AppConfigPath, "ExternalAudios", cwPath));
            File.Copy(Path.Combine(root, "audio", cwPath), ciPath, true);
            chanelSettings.NotificationSoundPath = ciPath;
            settings.NotificationChannelsNotifySettings[new Guid(channelId).ToString()] = chanelSettings;
        }
    }

    private void ImportClassWidgetsComponents(string root, IniData ini)
    {
        var profile = new ComponentProfile()
        {
            Lines = [
                new MainWindowLineSettings()
            ]
        };
        var line = profile.Lines[0];
        var widgets =
            ConfigureFileHelper.LoadConfigUnWrapped<CwWidgetsProfile>(Path.Combine(root, "config", "widget.json"));
        foreach (var widget in widgets.Widgets)
        {
            var comp = new ComponentSettings()
            {
                Id = widget switch
                {
                    "widget-weather.ui" => "CA495086-E297-4BEB-9603-C5C1C1A8551E",
                    "widget-time.ui" => "9E1AF71D-8F77-4B21-A342-448787104DD9",
                    "widget-next-activity.ui" => "E7831603-61A0-4180-B51B-54AD75B1A4D3",
                    "widget-countdown-day.ui" => "7C645D35-8151-48BA-B4AC-15017460D994",
                    _ => ""
                }
            };
            if (comp.Id == "")
            {
                continue;
            }
            line.Children.Add(comp);
        }

        var name = $"cw-{Guid.NewGuid()}.json";
        ConfigureFileHelper.SaveConfig(Path.Combine(ComponentsService.ComponentSettingsPath, name), profile);
        var settingsService = IAppHost.GetService<SettingsService>();
        var settings = settingsService.Settings;
        settings.CurrentComponentConfig = Path.GetFileNameWithoutExtension(name);
    }
    
    private async Task PerformClassWidgetsImportAction()
    {
        if (string.IsNullOrWhiteSpace(ViewModel.ImportSourcePath))
        {
            return;
        }
        var r = await new ContentDialog()
        {
            Title = "重启以继续",
            Content = "应用需要重启以完全应用导入操作，要继续吗？",
            PrimaryButtonText = "重启并继续",
            SecondaryButtonText = "取消",
            DefaultButton = ContentDialogButton.Primary
        }.ShowAsync(TopLevel.GetTopLevel(this));
        if (r != ContentDialogResult.Primary)
        {
            return;
        }
        var root = ViewModel.ImportSourcePath;
        try
        {
            ViewModel.PageIndex = 3;
            await Task.Run(() =>
            {
                var parser = new IniDataParser()
                {
                    Configuration =
                    {
                        SkipInvalidLines = true
                    }
                };
                var ini = parser.Parse(File.ReadAllText(Path.Combine(root, "config", "config.ini")));
                if (ViewModel.IsProfileSelected)
                {
                    ImportClassWidgetsProfile(root, ini);
                    var general = ini["General"];
                    var profileName = "cw_" + TryGetStringFromSection(general, "schedule", "新课表 - 1.json");
                    var settingsService = IAppHost.GetService<SettingsService>();
                    var settings = settingsService.Settings;
                    settings.SelectedProfile = profileName;
                }
                if (ViewModel.IsSettingsSelected)
                {
                    ImportClassWidgetsSettings(root, ini);
                }
                if (ViewModel.IsOtherConfigSelected)
                {
                    ImportClassWidgetsComponents(root, ini);
                }
            });
            AppBase.Current.Restart(["-m", "--importComplete"]);
        }
        catch (Exception e)
        {
            IAppHost.TryGetService<ILogger<DataTransferPage>>()?.LogError(e, "导入时发生意外错误");
            this.ShowErrorToast("导入时发生意外错误", e);
        }
    }

    #endregion

    private void ButtonFinish_OnClick(object? sender, RoutedEventArgs e)
    {
        (TopLevel.GetTopLevel(this) as Window)?.Close();
    }

    private void ButtonNext_OnClick(object? sender, RoutedEventArgs e)
    {
        ViewModel.PageIndex++;
    }
    
}
