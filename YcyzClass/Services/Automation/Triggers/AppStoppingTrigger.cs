using System;
using YcyzClass.Core;
using YcyzClass.Core.Abstractions.Automation;
using YcyzClass.Core.Attributes;

namespace YcyzClass.Services.Automation.Triggers;

[TriggerInfo("ycyzclass.lifetime.stopping", "应用退出时", "\ue0df")]
public class AppStoppingTrigger : TriggerBase
{
    public override void Loaded()
    {
        AppBase.Current.AppStopping += CurrentOnAppStarted;
    }

    public override void UnLoaded()
    {
        AppBase.Current.AppStopping -= CurrentOnAppStarted;
    }

    private void CurrentOnAppStarted(object? sender, EventArgs e)
    {
        Trigger();
    }
}