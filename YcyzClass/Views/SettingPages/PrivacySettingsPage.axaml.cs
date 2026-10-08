using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using YcyzClass.Core.Abstractions.Controls;
using YcyzClass.Core.Attributes;
using YcyzClass.Core.Controls;
using YcyzClass.Core.Enums.SettingsWindow;
using YcyzClass.Services;

namespace YcyzClass.Views.SettingPages;

/// <summary>
/// PrivacySettingsPage.xaml 的交互逻辑
/// </summary>
[Group("ycyzclass.general")]
[SettingsPageInfo("privacy", "隐私", "\uef65", "\uef64", SettingsPageCategory.Internal)]
public partial class PrivacySettingsPage : SettingsPageBase
{
    public SettingsService SettingsService { get; }

    public PrivacySettingsPage(SettingsService settingsService)
    {
        InitializeComponent();
        DataContext = this;
        SettingsService = settingsService;
    }

    private void HyperlinkMsAppCenter_OnClick(object sender, RoutedEventArgs e)
    {
        new DocumentReaderWindow()
        {
            Source = new Uri("avares://YcyzClass/Assets/Documents/Privacy_.md"),
            Title = "YcyzClass 隐私政策"
        }.ShowDialog((TopLevel.GetTopLevel(this) as Window)!);
    }
}
