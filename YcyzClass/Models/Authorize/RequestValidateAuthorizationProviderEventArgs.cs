using Avalonia.Interactivity;
using YcyzClass.Controls;
using YcyzClass.Views;

namespace YcyzClass.Models.Authorize;

public class RequestValidateAuthorizationProviderEventArgs(object? source) : RoutedEventArgs(AuthorizeProviderPresenter.RequestValidateAuthorizationProvidersEvent, source)
{
    public bool IsError { get; set; } = false;
}