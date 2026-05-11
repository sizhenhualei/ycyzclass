using System.Linq;
using System.Text.Json.Serialization;
using YcyzClass.Core.Attributes;
using YcyzClass.Core.Services.Registry;
using CommunityToolkit.Mvvm.ComponentModel;

namespace YcyzClass.Models.Authorize;

public partial class CredentialItem : ObservableObject
{
    [ObservableProperty] private string _providerId = "";

    [ObservableProperty] private object? _providerSettings;

    [ObservableProperty] private string _customName = "";

    [ObservableProperty] private bool _isCustomNameEnabled = false;

    [JsonIgnore]
    public AuthorizeProviderInfo? ProviderInfo =>
        AuthorizeProviderRegistryService.RegisteredAuthorizeProviders.FirstOrDefault(x =>
            x.Id == ProviderId);
}