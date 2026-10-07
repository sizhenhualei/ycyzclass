using System;
using YcyzClass.Core.Abstractions.Services;
using YcyzClass.Models.EventArgs;

namespace YcyzClass.Services.Automation.Triggers;

public class UriTriggerHandlerService
{
    private IUriNavigationService UriNavigationService { get; }

    internal event EventHandler<UriTriggerHandledEventArgs>? HandledRun;
    internal event EventHandler<UriTriggerHandledEventArgs>? HandledRevert;

    public UriTriggerHandlerService(IUriNavigationService uriNavigationService)
    {
        UriNavigationService = uriNavigationService;

        UriNavigationService.HandleAppNavigation("api/automation/run", args =>
        {
            HandledRun?.Invoke(this, new UriTriggerHandledEventArgs(string.Join('/', args.ChildrenPathPatterns)));
        });
        UriNavigationService.HandleAppNavigation("api/automation/revert", args =>
        {
            HandledRevert?.Invoke(this, new UriTriggerHandledEventArgs(string.Join('/', args.ChildrenPathPatterns)));
        });
    }
}