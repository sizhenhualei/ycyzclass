using YcyzClass.Core.Abstractions.Controls;
using YcyzClass.Core.Attributes;

namespace YcyzClass.ExamplePlugin.Views.SettingsPages;

[SettingsPageInfo("ycyzclass.example-plugin.hello", "Hello world!")]
public partial class HelloSettingsPage : SettingsPageBase
{
    public HelloSettingsPage()
    {
        InitializeComponent();
    }
}