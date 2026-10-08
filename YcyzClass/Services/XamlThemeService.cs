using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Styling;
using YcyzClass.Core;
using YcyzClass.Core.Abstractions.Services;
using YcyzClass.Core.Helpers;
using YcyzClass.Core.Models;
using YcyzClass.Core.Models.Plugin;
using YcyzClass.Core.Models.XamlTheme;
using YcyzClass.Shared;
using YcyzClass.Shared.ComponentModels;
using YcyzClass.Shared.Helpers;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace YcyzClass.Services;

public class XamlThemeService : ObservableRecipient, IXamlThemeService
{
    private static readonly FieldInfo? s_stylesAppliedField = typeof(StyledElement).GetField("_stylesApplied", BindingFlags.Instance | BindingFlags.NonPublic);
    
    public ILogger<XamlThemeService> Logger { get; }
    public SettingsService SettingsService { get; }
    private IComponentsService ComponentsService { get; }
    private Styles RootStyles { get; set; } = [];

    public Window? MainWindow { get; set; }
    
    private Border? ResourceLoaderBorder { get; set; }

    public static readonly string ThemesPath = Path.Combine(CommonDirectories.AppConfigPath, "Themes");
    public static readonly string EnabledThemesPath = Path.Combine(CommonDirectories.AppConfigPath, "EnabledThemes.json");
    public static readonly string ThemesPkgRootPath = Path.Combine(CommonDirectories.AppCacheFolderPath, "ThemePackages");

    public ObservableCollection<ThemeInfo> Themes { get; } = [];

    public ObservableDictionary<string, ThemeInfo> MergedThemes
    {
        get => _mergedThemes;
        set => SetProperty(ref _mergedThemes, value);
    }


    public ObservableCollection<string> EnabledThemes { get; }

    
    private ObservableDictionary<string, ThemeInfo> _mergedThemes = [];

    public double ActualVerticalSafeAreaPx { get; set; } = 0.0;


    public XamlThemeService(ILogger<XamlThemeService> logger,
        SettingsService settingsService, IComponentsService componentsService)
    {
        Logger = logger;
        SettingsService = settingsService;
        ComponentsService = componentsService;
        EnabledThemes = ConfigureFileHelper.LoadConfig<ObservableCollection<string>>(EnabledThemesPath);
        if (EnabledThemes.Count == 0)
        {
            EnabledThemes.Add("ycyzclass.fluent");
        }
        EnabledThemes.CollectionChanged +=
            (_, _) => ConfigureFileHelper.SaveConfig(EnabledThemesPath, EnabledThemes);
        if (App.ApplicationCommand.Safe)
        {
            return;
        }
        
        ProcessThemeInstall();
        // LoadAllThemes();
        //LoadThemeSource();
    }

    public void LoadAllThemes()
    {
        LoadThemeSource();

        if (App.ApplicationCommand.Safe)
        {
            return;
        }
        ResourceLoaderBorder ??= MainWindow?.FindControl<Border>("ResourceLoaderBorder");
        RootStyles.Clear();
        ResourceLoaderBorder?.Styles.Remove(RootStyles);
        s_stylesAppliedField?.SetValue(ResourceLoaderBorder, false); 
        RootStyles = [];
        ResourceLoaderBorder?.Styles.Add(RootStyles);
        var actualSafeAreaPx = 0.0;
        foreach (var themeInfo in EnabledThemes.Select(x => Themes.FirstOrDefault(y => y.Manifest.Id == x))
                     .OfType<ThemeInfo>())
        {
            try
            {
                if (themeInfo.IsExternal)
                {
                    LoadThemeFromFile(Path.Combine(themeInfo.Path, "Styles.axaml"));
                }
                else
                {
                    LoadThemeFromResource(themeInfo.ThemeUri ?? throw new InvalidOperationException("资源主题必须指定主题 Uri"));
                }
                actualSafeAreaPx = Math.Max(themeInfo.Manifest.VerticalSafeAreaPx, actualSafeAreaPx);
                themeInfo.IsLoaded = true;
            }
            catch (Exception e)
            {
                themeInfo.IsError = true;
                themeInfo.Error = e;
            }
        }

        ActualVerticalSafeAreaPx = actualSafeAreaPx;
    }

    private void LoadThemeFromFile(string themePath)
    {
        Logger.LogInformation("正在从文件加载主题 {}", themePath);
        var uri = new Uri(Path.GetFullPath(themePath));
        if (AvaloniaRuntimeXamlLoader.Load(File.ReadAllText(themePath), Assembly.GetExecutingAssembly(), uri: uri) is
            not Styles styles)
        {
            return;
        }
        RootStyles.Add(styles);
    }
    
    private void LoadThemeFromResource(Uri uri)
    {
        Logger.LogInformation("正在从资源加载主题 {}", uri);
        RootStyles.Add((IStyle)AvaloniaXamlLoader.Load(uri));
    }

    public void LoadThemeSource()
    {
        Logger.LogInformation("正在加载主题源");
        LoadLocalThemes();
        var merged = new ObservableDictionary<string, ThemeInfo>();
        
        foreach (var themeLocal in Themes)
        {
            var id = themeLocal.Manifest.Id;
            merged[id] = themeLocal;
        }


        MergedThemes = merged;
    }


    private void LoadLocalThemes()
    {
        Themes.Clear();
        foreach (var integratedTheme in IXamlThemeService.IntegratedThemes)
        {
            Themes.Add(integratedTheme);
        }
        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .IgnoreUnmatchedProperties()
            .Build();
        foreach (var i in Directory.GetDirectories(ThemesPath))
        {
            var manifest = new ThemeManifest()
            {
                Name = Path.GetFileName(i),
                Id = Path.GetFileName(i),
            };
            var themeInfo = new ThemeInfo
            {
                Path = Path.GetFullPath(i)
            };
            try
            {
                if (File.Exists(Path.Combine(i, "manifest.yml")))
                {
                    var yaml = File.ReadAllText(Path.Combine(i, "manifest.yml"));
                    manifest = deserializer.Deserialize<ThemeManifest>(yaml);
                }

                themeInfo.Manifest = manifest;
                themeInfo.Path = Path.GetFullPath(i);
                themeInfo.IsLocal = true;
                themeInfo.RealBannerPath = Path.GetFullPath(Path.Combine(themeInfo.Path, themeInfo.Manifest.Banner));
            }
            catch (Exception e)
            {
                themeInfo.IsError = true;
                themeInfo.Error = e;
                Logger.LogError(e, "无法加载主题元数据 {}", i);
            }
            Themes.Add(themeInfo);
        }
    }


    private void ProcessThemeInstall()
    {
        if (!Directory.Exists(ThemesPkgRootPath))
        {
            Directory.CreateDirectory(ThemesPkgRootPath);
        }
        if (!Directory.Exists(ThemesPath))
        {
            Directory.CreateDirectory(ThemesPath);
        }

        foreach (var pkgPath in Directory.EnumerateFiles(ThemesPkgRootPath).Where(x => Path.GetExtension(x) == ".zip"))
        {
            try
            {
                InstallTheme(pkgPath);
            }
            catch (Exception e)
            {
                Logger.LogError(e, "无法安装主题 {}", pkgPath);
            }
        }

        foreach (var pkg in Directory.EnumerateDirectories(ThemesPath).Where(x => Path.Exists(Path.Combine(x, ".uninstall"))))
        {
            try
            {
                Directory.Delete(pkg, true);
            }
            catch (Exception e)
            {
                Logger.LogError(e, "无法卸载主题 {}", pkg);
            }
        }
    }

    private static void InstallTheme(string pkgPath)
    {
        var deserializer = new DeserializerBuilder()
            .IgnoreUnmatchedProperties()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

        using (var pkg = ZipFile.OpenRead(pkgPath))
        {
            var mf = pkg.GetEntry("manifest.yml");
            if (mf == null)
                return;
            var mfText = new StreamReader(mf.Open()).ReadToEnd();
            var manifest = deserializer.Deserialize<PluginManifest>(mfText);
            var targetPath = Path.Combine(ThemesPath, manifest.Id);
            if (Directory.Exists(targetPath))
            {
                Directory.Delete(targetPath, true);
            }

            Directory.CreateDirectory(targetPath);
            ZipFile.ExtractToDirectory(pkgPath, targetPath);
        }
        File.Delete(pkgPath);
    }

    public async Task PackageThemeAsync(string id, string outputPath)
    {
        var plugin = Themes.FirstOrDefault(x => x.Manifest.Id == id);
        if (plugin == null)
        {
            throw new ArgumentException($"找不到主题 {id}。", nameof(id));
        }

        await Task.Run(() =>
        {
            if (File.Exists(outputPath))
                File.Delete(outputPath);
            ZipFile.CreateFromDirectory(plugin.Path, outputPath);
        });
    }
}