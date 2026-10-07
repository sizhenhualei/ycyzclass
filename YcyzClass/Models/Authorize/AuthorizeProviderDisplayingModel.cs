using Avalonia;
using YcyzClass.Core.Attributes;
using CommunityToolkit.Mvvm.ComponentModel;

namespace YcyzClass.Models.Authorize;

public partial class AuthorizeProviderDisplayingModel(AuthorizeProviderInfo info, Visual providerControl, CredentialItem associatedCredentialItem) : ObservableObject
{
    [ObservableProperty] private AuthorizeProviderInfo _info = info;

    [ObservableProperty] private Visual _providerControl = providerControl;

    [ObservableProperty] private CredentialItem _associatedCredentialItem = associatedCredentialItem;
}