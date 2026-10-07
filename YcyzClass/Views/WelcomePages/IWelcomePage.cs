using YcyzClass.ViewModels;

namespace YcyzClass.Views.WelcomePages;

public interface IWelcomePage
{
    WelcomeViewModel ViewModel { get; set; }
}