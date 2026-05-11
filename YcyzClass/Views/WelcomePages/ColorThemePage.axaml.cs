using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using YcyzClass.ViewModels;

namespace YcyzClass.Views.WelcomePages;

public partial class ColorThemePage : UserControl, IWelcomePage
{
    public ColorThemePage()
    {
        InitializeComponent();
    }

    public WelcomeViewModel ViewModel { get; set; } = null!;
}