using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using YcyzClass.ViewModels;

namespace YcyzClass.Views.WelcomePages;

public partial class SystemPage : UserControl, IWelcomePage
{
    public SystemPage()
    {
        InitializeComponent();
    }

    public WelcomeViewModel ViewModel { get; set; } = null!;
}