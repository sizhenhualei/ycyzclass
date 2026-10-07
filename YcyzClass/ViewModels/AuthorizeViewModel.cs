using System.Collections.ObjectModel;
using YcyzClass.Core.Attributes;
using YcyzClass.Models.Authorize;
using CommunityToolkit.Mvvm.ComponentModel;
using AuthorizeProviderDisplayingModel = YcyzClass.Models.Authorize.AuthorizeProviderDisplayingModel;

namespace YcyzClass.ViewModels;

public partial class AuthorizeViewModel : ObservableObject
{
    [ObservableProperty] private ObservableCollection<AuthorizeProviderDisplayingModel> _providers = [];

    [ObservableProperty] private bool _isEditingMode = false;

    [ObservableProperty] private Credential _credential = new();

    [ObservableProperty] private AuthorizeProviderInfo? _selectedAuthorizeProviderInfo;

    [ObservableProperty] private CredentialItem? _selectedCredentialItem;
}