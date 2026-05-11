using System.Diagnostics;
using System.Linq;
using YcyzClass.Core;
using YcyzClass.Core.Abstractions.Automation;
using YcyzClass.Core.Attributes;
using YcyzClass.Core.Enums;

namespace YcyzClass.Services.Automation.Triggers;

[TriggerInfo("ycyzclass.lifetime.startup", "应用启动时", "\ue067")]
public class AppStartupTrigger : TriggerBase
{
    public override void Loaded()
    {
        if (AppBase.CurrentLifetime < ApplicationLifetime.Running) {
            Trigger();
        }
    }

    public override void UnLoaded()
    {
    }
}