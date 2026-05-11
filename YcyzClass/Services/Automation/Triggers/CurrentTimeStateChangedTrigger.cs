using System;
using YcyzClass.Core.Abstractions.Automation;
using YcyzClass.Core.Abstractions.Services;
using YcyzClass.Core.Attributes;

namespace YcyzClass.Services.Automation.Triggers;

[TriggerInfo("ycyzclass.lessons.currentTimeStateChanged", "当前时间状态变化时", "\ue4d2")]
public class CurrentTimeStateChangedTrigger(ILessonsService lessonsService) : TriggerBase
{
    private ILessonsService LessonsService { get; } = lessonsService;

    public override void Loaded()
    {
        LessonsService.CurrentTimeStateChanged += CurrentLessonsServiceOnTimeStateChanged;
    }
    public override void UnLoaded()
    {
        LessonsService.CurrentTimeStateChanged -= CurrentLessonsServiceOnTimeStateChanged;
    }

    private void CurrentLessonsServiceOnTimeStateChanged(object? sender, EventArgs e)
    {
        Trigger();
    }
}