using YcyzClass.Core.Abstractions;
using YcyzClass.Core.Attributes;
using YcyzClass.Core.Extensions.Registry;
using YcyzClass.ExamplePlugin.Views.SettingsPages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace YcyzClass.ExamplePlugin;

[PluginEntrance]
public class Plugin : PluginBase
{
    public override void Initialize(HostBuilderContext context, IServiceCollection services)
    {
        Console.WriteLine("Hello world!");
        services.AddSettingsPage<HelloSettingsPage>();
    }
}