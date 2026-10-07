using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using YcyzClass.ViewModels;

namespace YcyzClass.Views.WelcomePages;

public partial class AppearancePage : UserControl, IWelcomePage
{
    public AppearancePage()
    {
        InitializeComponent();
    }

    public WelcomeViewModel ViewModel { get; set; } = null!;
}