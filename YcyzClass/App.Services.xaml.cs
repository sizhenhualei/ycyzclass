using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using YcyzClass.Controls.ActionSettingsControls;
using YcyzClass.Controls.AttachedSettingsControls;
using YcyzClass.Controls.AuthorizeProvider;
using YcyzClass.Controls.Components;
using YcyzClass.Controls.EditMode;
using YcyzClass.Controls.NotificationProviders;
using YcyzClass.Controls.ProfileTransferProviders;
using YcyzClass.Controls.RuleSettingsControls;
using YcyzClass.Controls.SpeechProviderSettingsControls;
using YcyzClass.Controls.TriggerSettingsControls;
using YcyzClass.Core.Abstractions.Services;
using YcyzClass.Core.Abstractions.Services.Management;
using YcyzClass.Core.Abstractions.Services.Metadata;
using YcyzClass.Core.Abstractions.Services.SpeechService;
using YcyzClass.Core.Controls.Ruleset;
using YcyzClass.Core.Enums.Profile;
using YcyzClass.Core.Extensions.Registry;
using YcyzClass.Core.Helpers;
using YcyzClass.Core.Models.Logging;
using YcyzClass.Core.Models.Ruleset;
using YcyzClass.Core.Models.XamlTheme;
using YcyzClass.Helpers.ProfileTransferHelpers;
using YcyzClass.Models.Actions;
using YcyzClass.Models.Automation.Triggers;
using YcyzClass.Models.Rules;
using YcyzClass.Platforms.Abstraction;
using YcyzClass.Platforms.Abstraction.Services;
using YcyzClass.Services;
using YcyzClass.Services.AppUpdating;
using YcyzClass.Services.Automation.Actions;
using YcyzClass.Services.Automation.Triggers;
using YcyzClass.Services.Logging;
using YcyzClass.Services.Management;
using YcyzClass.Services.Metadata;
using YcyzClass.Services.NotificationProviders;
using YcyzClass.Services.SpeechService;
using YcyzClass.ViewModels;
using YcyzClass.ViewModels.EditMode;
using YcyzClass.ViewModels.SettingsPages;
using YcyzClass.Views;
using YcyzClass.Views.SettingPages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Sentry;

namespace YcyzClass;

public partial class App
{
    private void ConfigureServices(HostBuilderContext context, IServiceCollection services)
    {
        services.AddSingleton<SettingsService>();
        services.AddSingleton<UpdateService>();
        services.AddSingleton<ITaskBarIconService, TaskBarIconService>();
        // services.AddSingleton<WallpaperPickingService>();
        services.AddSingleton<INotificationHostService, NotificationHostService>();
        services.AddSingleton<INotificationWorkerService, NotificationWorkerService>();
        services.AddSingleton<IThemeService, ThemeService>();
        services.AddSingleton<MiniInfoProviderHostService>();
        services.AddSingleton<IWeatherService, WeatherService>();
        services.AddSingleton<FileFolderService>();
        services.AddSingleton<IAttachedSettingsHostService, AttachedSettingsHostService>();
        services.AddSingleton<IProfileService, ProfileService>();
        services.AddSingleton<ISplashService, SplashService>();
        services.AddSingleton<IHangService, HangService>();
        services.AddSingleton<ConsoleService>();
        //services.AddHostedService<BootService>();
        services.AddSingleton<DiagnosticService>();
        services.AddSingleton<IManagementService, ManagementService>();
        services.AddSingleton<AppLogService>();
        services.AddSingleton<IComponentsService, ComponentsService>();
        services.AddSingleton<ILessonsService, LessonsService>();
        services.AddSingleton<IUriNavigationService, UriNavigationService>();
        services.AddHostedService<MemoryWatchDogService>();
        services.AddSingleton<IPluginService, PluginService>();
        services.AddSingleton<IPluginMarketService, PluginMarketService>();
        services.AddSingleton<IRulesetService, RulesetService>();
        services.AddSingleton<IActionService, ActionService>();
        services.AddSingleton<IWindowRuleService, WindowRuleService>();
        services.AddSingleton<IAutomationService, AutomationService>();
        services.AddSingleton<ISpeechService>(GetSpeechService);
        services.AddSingleton<IExactTimeService, ExactTimeService>();
        //services.AddSingleton(typeof(ApplicationCommand), ApplicationCommand);
        services.AddSingleton<IProfileAnalyzeService, ProfileAnalyzeService>();
        services.AddSingleton<IIpcService, IpcService>();
        services.AddSingleton<IAuthorizeService, AuthorizeService>();
        services.AddSingleton<UriTriggerHandlerService>();
        services.AddSingleton<SignalTriggerHandlerService>();
        services.AddSingleton<TrayMenuTriggerHandlerService>();
        services.AddSingleton<IAnnouncementService, AnnouncementService>();
        services.AddSingleton<ILocationService>(PlatformServices.LocationService);
        services.AddSingleton<IXamlThemeService, XamlThemeService>();
        services.AddSingleton<IAudioService, AudioService>();
        services.AddSingleton<ITutorialService, TutorialService>();
        services.AddSingleton<IRefreshingService, RefreshingService>();
        // ViewModels
        services.AddTransient<ProfileSettingsViewModel>();
        services.AddTransient<DevPortalViewModel>();
        services.AddTransient<AppLogsViewModel>();
        services.AddTransient<WelcomeViewModel>();
        services.AddTransient<ClassChangingViewModel>();
        services.AddTransient<DataTransferViewModel>();
        services.AddTransient<ScreenshotHelperViewModel>();
        services.AddTransient<EditModeViewModel>();
        services.AddTransient<TutorialEditorViewModel>();
        services.AddTransient<TutorialCenterViewModel>();
        // ViewModels/SettingsPages
        services.AddTransient<GeneralSettingsViewModel>();
        services.AddTransient<ClockSettingsViewModel>();
        services.AddTransient<AdvancedSettingsViewModel>();
        services.AddTransient<AboutSettingsViewModel>();
        services.AddTransient<AppearanceSettingsViewModel>();
        services.AddTransient<ComponentsSettingsViewModel>();
        services.AddTransient<NotificationSettingsViewModel>();
        services.AddTransient<WindowSettingsViewModel>();
        services.AddTransient<WeatherSettingsViewModel>();
        services.AddTransient<AutomationSettingsViewModel>();
        services.AddTransient<PluginsSettingsPageViewModel>();
        services.AddTransient<StorageSettingsViewModel>();
        services.AddTransient<ErrorSettingsViewModel>();
        services.AddTransient<ThemesSettingsViewModel>();
        services.AddTransient<UpdateSettingsPageViewModel>();
        services.AddTransient<DebugPageViewModel>();
        services.AddTransient<RefreshingSettingsViewModel>();
        // Views
        services.AddTransient<ITopmostEffectPlayer>(x => x.GetRequiredService<TopmostEffectWindow>());
        services.AddSingleton<MainWindow>();
        // services.AddTransient<SplashWindowBase, SplashWindow>();
        // services.AddTransient<FeatureDebugWindow>();
        services.AddSingleton<TopmostEffectWindow>();
        services.AddSingleton<AppLogsWindow>();
        services.AddSingleton<SettingsWindowNew>();
        services.AddSingleton<ProfileSettingsWindow>();
        services.AddTransient<ClassPlanDetailsWindow>();
        services.AddTransient<WindowRuleDebugWindow>();
        // services.AddTransient<ConfigErrorsWindow>();
        services.AddTransient<TimeAdjustmentWindow>();
        // services.AddTransient<ExcelExportWindow>();
        services.AddTransient<DevPortalWindow>();
        services.AddTransient<WelcomeWindow>();
        services.AddTransient<DataTransferWindow>();
        services.AddTransient<ScreenshotHelperWindow>();
        services.AddSingleton<EditModeView>();
        services.AddTransient<TutorialEditorWindow>();
        services.AddSingleton<TutorialCenterWindow>();
        services.AddTransient<TutorialControllerWindow>();
        services.AddTransient<ISplashProvider, SplashWindow>();
        // 设置页面分组
        services.AddSettingsPageGroup("ycyzclass.general", "\uef27", "通用");
        services.AddSettingsPageGroup("ycyzclass.mainwindow", "\uec85", "主界面");
        // 设置页面
        services.AddSettingsPage<GeneralSettingsPage>();
        services.AddSettingsPage<ClockSettingsPage>();
        services.AddSettingsPage<StorageSettingsPage>();
        services.AddSettingsPage<PrivacySettingsPage>();
        services.AddSettingsPage<RefreshingSettingsPage>();
        services.AddSettingsPage<AdvancedSettingsPage>();
        services.AddSettingsPage<ComponentsSettingsPage>();
        services.AddSettingsPage<AppearanceSettingsPage>();
        services.AddSettingsPage<NotificationSettingsPage>();
        services.AddSettingsPage<WindowSettingsPage>();
        services.AddSettingsPage<WeatherSettingsPage>();
        services.AddSettingsPage<AutomationSettingsPage>();
        if (UpdateService.AllowedPackageTypes.Contains(PackagingType))
        {
            services.AddSettingsPage<UpdateSettingsPage>();
        }
        services.AddSettingsPage<PluginsSettingsPage>();
        services.AddSettingsPage<ThemesSettingsPage>();
        services.AddSettingsPage<TestSettingsPage>();
        services.AddSettingsPage<DebugPage>();
        // services.AddSettingsPage<DebugBrushesSettingsPage>();
        services.AddSettingsPage<AboutSettingsPage>();
        services.AddSettingsPage<ManagementSettingsPage>();
        services.AddSettingsPage<ManagementCredentialsSettingsPage>();
        services.AddSettingsPage<ManagementPolicySettingsPage>();
        services.AddSettingsPage<ErrorSettingsPage>();
        // 主界面组件
        services.AddComponent<TextComponent, TextComponentSettingsControl>();
        services.AddComponent<SeparatorComponent>();
        services.AddComponent<ScheduleComponent, ScheduleComponentSettingsControl>();
        services.AddComponent<DateComponent>();
        services.AddComponent<ClockComponent, ClockComponentSettingsControl>();
        services.AddComponent<WeatherComponent, WeatherComponentSettingsControl>();
        services.AddComponent<CountDownComponent, CountDownComponentSettingsControl>();
        services.AddComponent<SlideComponent, SlideComponentSettingsControl>();
        services.AddComponent<RollingComponent, RollingComponentSettingsControl>();
        services.AddComponent<GroupComponent>();
        services.AddComponent<StackComponent>();
        // 提醒提供方
        services.AddNotificationProvider<ClassNotificationProvider, ClassNotificationProviderSettingsControl>();
        services.AddNotificationProvider<AfterSchoolNotificationProvider, AfterSchoolNotificationProviderSettingsControl>();
        services.AddNotificationProvider<WeatherNotificationProvider, WeatherNotificationProviderSettingsControl>();
        services.AddNotificationProvider<ManagementNotificationProvider>();
        services.AddNotificationProvider<ActionNotificationProvider>();
        // // Transients
        // services.AddTransient<ExcelImportWindow>();
        // services.AddTransient<WallpaperPreviewWindow>();
        // Logging
        services.AddLogging(builder =>
        {
            LogMaskingHelper.Rules.Add(new LogMaskRule(new(@"(latitude=)(\d*\.?\d*)"), 2));
            LogMaskingHelper.Rules.Add(new LogMaskRule(new(@"(longitude=)(\d*\.?\d*)"), 2));

            builder.AddConsoleFormatter<YcyzClassConsoleFormatter, ConsoleFormatterOptions>();
            builder.AddConsole(console => { console.FormatterName = "ycyzclass"; });
            builder.AddSentry(o =>
            {
                o.InitializeSdk = false;
                o.MinimumBreadcrumbLevel = LogLevel.Information;
                o.EnableLogs = true;
                o.SetBeforeSendLog(log => log.Level < SentryLogLevel.Info ? null : log);
            });
            var debug = false;
#if DEBUG
            debug = true;
#endif
            if (ApplicationCommand.Verbose || debug)
            {
                builder.SetMinimumLevel(LogLevel.Trace);
            }
        });
        services.AddSingleton<ILoggerProvider, SentryLoggerProvider>();
        services.AddSingleton<ILoggerProvider, AppLoggerProvider>();
        services.AddSingleton<ILoggerProvider, FileLoggerProvider>();
        // AttachedSettings
        services.AddAttachedSettingsControl<AfterSchoolNotificationAttachedSettingsControl>();
        services.AddAttachedSettingsControl<ClassNotificationAttachedSettingsControl>();
        services.AddAttachedSettingsControl<LessonControlAttachedSettingsControl>();
        services.AddAttachedSettingsControl<WeatherNotificationAttachedSettingsControl>();
        // 触发器
        services.AddTrigger<RulesetChangedTrigger>();
        services.AddTrigger<SignalTrigger, SignalTriggerSettingsControl>();
        services.AddTrigger<UriTrigger, UriTriggerSettingsControl>();
        services.AddTrigger<TrayMenuTrigger, TrayMenuTriggerSettingsControl>();
        services.AddTrigger<CronTrigger, CronTriggerSettingsControl>();
        services.AddTrigger<AppStartupTrigger>();
        services.AddTrigger<AppStoppingTrigger>();
        services.AddTrigger<OnClassTrigger>();
        services.AddTrigger<OnBreakingTimeTrigger>();
        services.AddTrigger<OnAfterSchoolTrigger>();
        services.AddTrigger<CurrentTimeStateChangedTrigger>();
        services.AddTrigger<PreTimePointTrigger, PreTimePointTriggerSettingsControl>();
        // 规则
        services.AddRule("ycyzclass.test.true", "总是为真", onHandle: _ => true);
        services.AddRule("ycyzclass.test.false", "总是为假", onHandle: _ => false);
        services.AddRule<StringMatchingSettings, RulesetStringMatchingSettingsControl>("ycyzclass.windows.className", "前台窗口类名", "\uF4A2");
        services.AddRule<StringMatchingSettings, RulesetStringMatchingSettingsControl>("ycyzclass.windows.text", "前台窗口标题", "\uF26B");
        services.AddRule<WindowStatusRuleSettings, WindowStatusRuleSettingsControl>("ycyzclass.windows.status", "前台窗口状态是", "\uEC83");
        services.AddRule<StringMatchingSettings, RulesetStringMatchingSettingsControl>("ycyzclass.windows.processName", "前台窗口进程", "\uF488");
        services.AddRule<CurrentSubjectRuleSettings, CurrentSubjectRuleSettingsControl>("ycyzclass.lessons.currentSubject", "科目是", "\uE215");
        services.AddRule<CurrentSubjectRuleSettings, CurrentSubjectRuleSettingsControl>("ycyzclass.lessons.nextSubject", "下节课科目是", "\uE217");
        services.AddRule<CurrentSubjectRuleSettings, CurrentSubjectRuleSettingsControl>("ycyzclass.lessons.previousSubject", "上节课科目是", "\uE226");
        services.AddRule<TimeStateRuleSettings, TimeStateRuleSettingsControl>("ycyzclass.lessons.timeState", "当前时间状态是", "\uE4C4");
        services.AddRule<CurrentWeatherRuleSettings, CurrentWeatherRuleSettingsControl>("ycyzclass.weather.currentWeather", "当前天气是", "\uE4DC");
        services.AddRule<CurrentWeatherRuleSettings, CurrentWeatherRuleSettingsControl>("ycyzclass.weather.tomorrowWeather", "明天天气是", "\uE4DC");
        services.AddRule<StringMatchingSettings, RulesetStringMatchingSettingsControl>("ycyzclass.weather.hasWeatherAlert", "存在气象预警", "\uF431");
        services.AddRule<RainTimeRuleSettings, RainTimeRuleSettingsControl>("ycyzclass.weather.rainTime", "距离降水开始/结束还剩", "\uF43F");
        services.AddRule<SunRiseSetRuleSettings, SunRiseSetRuleSettingsControl>("ycyzclass.weather.sunRiseSet", "是否日出/日落", "\uE150");
        // 行动提供方
        services.AddAction<SignalTriggerSettings, BroadcastSignalActionSettingsControl>("ycyzclass.broadcastSignal", "广播信号", "\uE561");
        services.AddAction<RunAction, RunActionSettingsControl>();
        services.AddAction<NotificationAction, NotificationActionSettingsControl>();
        services.AddAction<SleepAction, SleepActionSettingsControl>();
        services.AddAction<ModifyAppSettingsAction, ModifyAppSettingsActionSettingsControl>();
        services.AddAction<WeatherNotificationAction, WeatherNotificationActionSettingControl>();
        services.AddAction<AppQuitAction>();
        services.AddAction<AppRestartAction, AppRestartActionSettingsControl>();

        // 认证提供方
        services.AddAuthorizeProvider<PasswordAuthorizeProvider>();
        // 语音提供方
        if (System.OperatingSystem.IsWindows()) {
            services.AddSpeechProvider<SystemSpeechService>();
        }
        services.AddSpeechProvider<EdgeTtsService, EdgeTtsSpeechServiceSettingsControl>();
        services.AddSpeechProvider<GptSoVitsService, GptSovitsSpeechServiceSettingsControl>();
        // 天气图标模板
        services.AddWeatherIconTemplate("ycyzclass.weatherIcons.lucide", "Lucide（默认）", (this.FindResource("LucideWeatherIconTemplate") as IDataTemplate)!);
        services.AddWeatherIconTemplate("ycyzclass.weatherIcons.fluentDesign", "Fluent Design", (this.FindResource("FluentDesignWeatherIconTemplate") as IDataTemplate)!);
        services.AddWeatherIconTemplate("ycyzclass.weatherIcons.sfSymbols", "SF Symbols", (this.FindResource("SFSymbolsWeatherIconTemplate") as IDataTemplate)!);
        services.AddWeatherIconTemplate("ycyzclass.weatherIcons.simpleText", "纯文本", (this.FindResource("SimpleTextWeatherIconTemplate") as IDataTemplate)!);
        // 档案迁移提供方
        services.AddProfileTransferProvider<CsesImportProvider>("ycyzclass.profileTransfer.import.cses", "从 CSES 导入", ProfileTransferProviderType.Import, "\ue6cb");
        services.AddProfileTransferProvider<YcyzClass1ImportProvider>("ycyzclass.profileTransfer.import.legacyV1", "从 YcyzClass 1.x 导入", ProfileTransferProviderType.Import, "bitmap(avares://YcyzClass/Assets/AppLogo.png)");
        services.AddProfileTransferProvider<ClassWidgets1ImportProvider>("ycyzclass.profileTransfer.import.classWidgets", "从 Class Widgets 1.2 导入", ProfileTransferProviderType.Import);
        services.AddProfileTransferProvider("ycyzclass.profileTransfer.export.cses", "导出到 CSES", ProfileTransferProviderType.Export, CsesExportHelper.CsesExportHandler, "\ue6cb");
        // Themes
        services.AddXamlTheme(new Uri("avares://YcyzClass/XamlThemes/ClassicTheme/Styles.axaml"), new ThemeManifest()
        {
            Id = "ycyzclass.classic",
            Name = "经典",
            Description = "YcyzClass 的经典外观。",
            Banner = "avares://YcyzClass/Assets/XamlThemePreviews/ycyzclass.classic.png",
            Author = "YcyzClass",
            Url = "https://github.com/YcyzClass/YcyzClass"
        });
        services.AddXamlTheme(new Uri("avares://YcyzClass/XamlThemes/FluentTheme/Styles.axaml"), new ThemeManifest()
        {
            Id = "ycyzclass.fluent",
            Name = "Fluent",
            Description = "焕然一新的 YcyzClass 外观。",
            Banner = "avares://YcyzClass/Assets/XamlThemePreviews/ycyzclass.fluent.png",
            Author = "YcyzClass",
            Url = "https://github.com/YcyzClass/YcyzClass",
            VerticalSafeAreaPx = 20
        });
        // 教程
        // services.AddTutorialGroupByUri(new Uri("avares://YcyzClass/Assets/Tutorials/ycyzclass.test.json"));
        // services.AddTutorialGroupByUri(new Uri("avares://YcyzClass/Assets/Tutorials/ycyzclass.sp.json"));
        services.AddTutorialGroupByUri(new Uri("avares://YcyzClass/Assets/Tutorials/ycyzclass.getStarted.json"));
        // Plugins
        if (!ApplicationCommand.Safe && string.IsNullOrWhiteSpace(ApplicationCommand.ImportV1))
        {
            PluginService.InitializePlugins(context, services);
        }
    }
}